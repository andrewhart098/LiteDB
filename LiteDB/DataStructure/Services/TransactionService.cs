﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LiteDB
{
    /// <summary>
    /// Manage all transaction and garantee concurrency and recovery
    /// </summary>
    internal class TransactionService
    {
        private IDiskService _disk;
        private CacheService _cache;
        private bool _trans = false;

        internal TransactionService(IDiskService disk, CacheService cache)
        {
            _disk = disk;
            _cache = cache;
            _cache.MarkAsDirtyAction = (page) => _disk.WriteJournal(page.PageID, page.DiskData);
        }

        /// <summary>
        /// Starts a new transaction - lock database to garantee that only one processes is in a transaction
        /// </summary>
        public void Begin()
        {
            if(_trans == true) throw new SystemException("Begin transaction");

            this.AvoidDirtyRead();

            // lock (or try to) datafile
            _disk.Lock();

            _trans = true;
        }

        /// <summary>
        /// Commit the transaction - increese 
        /// </summary>
        public void Commit()
        {
            if (_trans == false) throw new SystemException("Commit transaction");

            if (_cache.HasDirtyPages)
            {
                var header = _cache.GetPage<HeaderPage>(0);

                // increase file changeID (back to 0 when overflow)
                header.ChangeID = header.ChangeID == ushort.MaxValue ? (ushort)0 : (ushort)(header.ChangeID + (ushort)1);

                // add header page as dirty to cache
                _cache.AddPage(header, true);

                // commit journal file - it will be used if write operation fails
                _disk.CommitJournal((header.LastPageID + 1) * BasePage.PAGE_SIZE);

                // write all dirty pages in data file
                foreach (var page in _cache.GetDirtyPages())
                {
                    //Console.WriteLine("save page " + page.PageID);
                    _disk.WritePage(page.PageID, page.WritePage());

                    if(page.PageID == 800)
                    {
                        Console.WriteLine("parar");
                    }
                }

                // delete journal file - datafile is consist here
                _disk.DeleteJournal();

                // set all dirty pages as clear on cache
                _cache.ClearDirty();
            }

            // unlock datafile
            _disk.Unlock();

            _trans = false;
        }

        public void Rollback()
        {
            if (_trans == false) throw new SystemException("Rollback transaction");

            // clear all pages from memory (return true if has dirty pages on cache)
            if (_cache.Clear())
            {
                // if has dirty page, has journal file - delete it (is not valid)
                _disk.DeleteJournal();
            }

            // unlock datafile
            _disk.Unlock();

            _trans = false;
        }

        /// <summary>
        /// This method must be called before read/write operation to avoid dirty reads.
        /// It's occurs when my cache contains pages that was changed in another process
        /// </summary>
        public void AvoidDirtyRead()
        {
            lock (_disk)
            {
                // if is in transaction pages are not dirty (begin trans was checked)
                if (_trans == true) return;

                var cache = _cache.GetPage<HeaderPage>(0);

                if (cache == null) return;

                // read change direct from disk
                var change = _disk.GetChangeID();

                // if changeID was changed, file was changed by another process - clear all cache
                if (cache.ChangeID != change)
                {
                    //Console.WriteLine("Datafile changed, clear cache");
                    _cache.Clear();
                }
            }
        }
    }
}
