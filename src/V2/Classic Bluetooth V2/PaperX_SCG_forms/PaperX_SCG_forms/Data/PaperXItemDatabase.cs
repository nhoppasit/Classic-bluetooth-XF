using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperX_SCG_forms
{
    public class PaperXItemDatabase
    {
        static readonly Lazy<SQLiteAsyncConnection> lazyInitializer = new Lazy<SQLiteAsyncConnection>(() =>
        {
            return new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        });

        static SQLiteAsyncConnection Database => lazyInitializer.Value;
        static bool initialized = false;

        public PaperXItemDatabase()
        {
            InitializeAsync().SafeFireAndForget(false);
        }

        async Task InitializeAsync()
        {
            if (!initialized)
            {
                if (!Database.TableMappings.Any(m => m.MappedType.Name == typeof(PaperXItem).Name))
                {
                    await Database.CreateTablesAsync(CreateFlags.None, typeof(PaperXItem)).ConfigureAwait(false);
                    initialized = true;
                }
            }
        }

        public Task<List<PaperXItem>> GetItemsAsync()
        {
            //  return Database.Table<PaperXItem>().ToListAsync();
            return Database.QueryAsync<PaperXItem>("SELECT * FROM [PaperXItem] ORDER BY [transactionDate] DESC");
        }

        public Task<List<PaperXItem>> GetItemsNotSendDataAsync()
        {
            return Database.QueryAsync<PaperXItem>("SELECT * FROM [PaperXItem] WHERE [statusSaved] = True and [statusSend] = False ORDER BY [transactionDate] DESC");
        }

        public Task<List<PaperXItem>> GetWeightLastDate()
        {
            return Database.QueryAsync<PaperXItem>("SELECT * FROM [PaperXItem] ORDER BY [transactionDate] DESC LIMIT 1");
        }

        public Task<PaperXItem> GetItemAsync(int id)
        {
            return Database.Table<PaperXItem>().Where(i => i.ID == id).FirstOrDefaultAsync();
        }

        public Task<int> SaveItemAsync(PaperXItem item)
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

        public Task<int> DeleteItemAsync(PaperXItem item)
        {
            return Database.DeleteAsync(item);
        }

        public Task<int> DeleteItemAllAsync()
        {
            return Database.ExecuteAsync("delete from [PaperXItem]");
        }
    }
}
