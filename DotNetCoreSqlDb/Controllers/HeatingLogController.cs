using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DotNetCoreSqlDb;
using DotNetCoreSqlDb.Data;
using DotNetCoreSqlDb.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace DotNetCoreSqlDb.Controllers
{
    [ActionTimerFilter]
    public class HeatingLogController : Controller
    {
        private readonly MyDatabaseContext _context;
        private readonly IDistributedCache _cache;
        private readonly string _HeatingLogItemsCacheKey = "HeatingLogItemsList";

        public HeatingLogController(MyDatabaseContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        // GET: HeatingLog
        public async Task<IActionResult> Index()
        {
            var heatingLogs = new List<HeatingLog>();
            byte[]? TodoListByteArray;

            TodoListByteArray = await _cache.GetAsync(_HeatingLogItemsCacheKey);
            if (TodoListByteArray != null && TodoListByteArray.Length > 0)
            { 
                heatingLogs = ConvertData<HeatingLog>.ByteArrayToObjectList(TodoListByteArray);
            }
            else 
            {
                heatingLogs = await _context.HeatingLogs.ToListAsync();
                TodoListByteArray = ConvertData<HeatingLog>.ObjectListToByteArray(heatingLogs);
                await _cache.SetAsync(_HeatingLogItemsCacheKey, TodoListByteArray);
            }

            return View(heatingLogs);
        }

        // GET: HeatingLog/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            byte[]? heatingLogItemByteArray;
            HeatingLog? heatingLog;

            if (id == null)
            {
                return NotFound();
            }

            heatingLogItemByteArray = await _cache.GetAsync(GetTodoItemCacheKey(id));

            if (heatingLogItemByteArray != null && heatingLogItemByteArray.Length > 0)
            {
                heatingLog = ConvertData<HeatingLog>.ByteArrayToObject(heatingLogItemByteArray);
            }
            else 
            {
                heatingLog = await _context.HeatingLogs
                .FirstOrDefaultAsync(m => m.ID == id);
            if (heatingLog == null)
            {
                return NotFound();
            }

                heatingLogItemByteArray = ConvertData<HeatingLog>.ObjectToByteArray(heatingLog);
                await _cache.SetAsync(GetTodoItemCacheKey(id), heatingLogItemByteArray);
            }

            

            return View(heatingLog);
        }

        // GET: HeatingLog/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: HeatingLog/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Comment,CreatedDate")] HeatingLog heatingLog)
        {
            if (ModelState.IsValid)
            {
                _context.Add(heatingLog);
                await _context.SaveChangesAsync();
                await _cache.RemoveAsync(_HeatingLogItemsCacheKey);
                return RedirectToAction(nameof(Index));
            }
            return View(heatingLog);
        }

        // GET: HeatingLog/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var heatingLog = await _context.HeatingLogs.FindAsync(id);
            if (heatingLog == null)
            {
                return NotFound();
            }
            return View(heatingLog);
        }

        // POST: HeatingLog/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Comment,CreatedDate")] HeatingLog heatingLog)
        {
            if (id != heatingLog.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(heatingLog);
                    await _context.SaveChangesAsync();
                    await _cache.RemoveAsync(GetTodoItemCacheKey(heatingLog.ID));
                    await _cache.RemoveAsync(_HeatingLogItemsCacheKey);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TodoExists(heatingLog.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(heatingLog);
        }

        // GET: HeatingLog/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var heatinglog = await _context.HeatingLogs
                .FirstOrDefaultAsync(m => m.ID == id);
            if (heatinglog == null)
            {
                return NotFound();
            }

            return View(heatinglog);
        }

        // POST: HeatingLog/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var heatingLog = await _context.HeatingLogs.FindAsync(id);
            if (heatingLog != null)
            {
                _context.HeatingLogs.Remove(heatingLog);
                await _context.SaveChangesAsync();
                await _cache.RemoveAsync(GetTodoItemCacheKey(heatingLog.ID));
                await _cache.RemoveAsync(_HeatingLogItemsCacheKey);
            }
            return RedirectToAction(nameof(Index));
        }

        private bool TodoExists(int id)
        {
            return _context.HeatingLogs.Any(e => e.ID == id);
        }

        private string GetTodoItemCacheKey(int? id)
        {
            return _HeatingLogItemsCacheKey+"_&_"+id;
        }
    }

    
}
