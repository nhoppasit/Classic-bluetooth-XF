using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperX_SCG_forms
{
   public class LogDatabase
    {
        static readonly Lazy<SQLiteAsyncConnection> lazyInitializer = new Lazy<SQLiteAsyncConnection>(() =>
        {
            return new SQLiteAsyncConnection(Constants.DatabasePathLog, Constants.Flags);
        });

        static SQLiteAsyncConnection Database => lazyInitializer.Value;
        static bool initialized = false;

        public LogDatabase()
        {
            InitializeAsync().SafeFireAndForget(false);
        }

        async Task InitializeAsync()
        {
            if (!initialized)
            {
                if (!Database.TableMappings.Any(m => m.MappedType.Name == typeof(LogItem).Name))
                {
                    await Database.CreateTablesAsync(CreateFlags.None, typeof(LogItem)).ConfigureAwait(false);
                    initialized = true;
                }
            }
        }

        public Task<List<LogItem>> GetItemsAsync()
        {
            return Database.Table<LogItem>().ToListAsync();
        }

        public Task<List<LogItem>> GetItemsAllSortDateDESC()
        {
            return Database.QueryAsync<LogItem>("SELECT * FROM [LogItem] ORDER BY [DateTimeLog] DESC");
        }

        //public Task<List<LogItem>> GetWeightLastDate()
        //{
        //    return Database.QueryAsync<LogItem>("SELECT * FROM [LogItem] ORDER BY [transactionDate] DESC LIMIT 1");
        //}

        public Task<LogItem> GetItemAsync(int id)
        {
            return Database.Table<LogItem>().Where(i => i.ID == id).FirstOrDefaultAsync();
        }

        public Task<int> SaveItemAsync(LogItem item)
        {
            if (item.ID != 0)
            {
                return Database.UpdateAsync(item);
            }
            else
            {
                return Database.InsertAsync(item);
            }
        }

        public Task<int> DeleteItemAsync(LogItem item)
        {
            return Database.DeleteAsync(item);
        }

        public Task<int> DeleteItemAllAsync()
        {
            return Database.ExecuteAsync("delete from [LogItem]");
        }
    }
}
