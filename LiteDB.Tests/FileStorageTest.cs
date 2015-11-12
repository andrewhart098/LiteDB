using System;
using System.Linq;
using Xunit;
using LiteDB;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;

namespace UnitTest
{
    public class FileStorage_Test
    {
        [Fact]
        public void FileStorage_InsertDelete()
        {
            // create a dump file
            File.WriteAllText("Core.dll", "FileCoreContent");

            using (var db = new LiteDatabase(DB.Path()))
            {
                db.FileStorage.Upload("Core.dll", "Core.dll");

                var exists = db.FileStorage.Exists("Core.dll");

                Assert.Equal(true, exists);

                var deleted = db.FileStorage.Delete("Core.dll");

                Assert.Equal(true, deleted);

                var deleted2 = db.FileStorage.Delete("Core.dll");

                Assert.Equal(false, deleted2);


            }

            File.Delete("Core.dll");
        }

        public string fdb = DB.Path();
        public Random rnd = new Random();

        [Fact]
        public void FileStorage_Concurrency()
        {
            using (var db = new LiteDatabase(fdb))
            {
            }

            var t1 = new Thread(new ThreadStart(TaskInsert));
            var t2 = new Thread(new ThreadStart(TaskInsert));
            var t3 = new Thread(new ThreadStart(TaskInsert));
            var t4 = new Thread(new ThreadStart(TaskInsert));
            var t5 = new Thread(new ThreadStart(TaskInsert));
            var t6 = new Thread(new ThreadStart(TaskInsert));

            t1.Start();
            t2.Start();
            t3.Start();
            t4.Start();
            t5.Start();
            t6.Start();

            t1.Join();
            t2.Join();
            t3.Join();
            t4.Join();
            t5.Join();
            t6.Join();

        }

        public void TaskInsert()
        {
            var file = new byte[1 * 1024 * 1025]; // 30MB

            System.Threading.Thread.Sleep(rnd.Next(100));

            using (var db = new LiteDatabase(fdb))
            {
                db.FileStorage.Upload(Guid.NewGuid().ToString(), new MemoryStream(file));
            }
        }
    }
}
