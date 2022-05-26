using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;
using System.Linq;
using Notes.Models;
using System;

namespace Notes.Data
{
    public class NoteDatabase
    {
        readonly SQLiteAsyncConnection database;

        public NoteDatabase(string dbPath)
        {
            database = new SQLiteAsyncConnection(dbPath);
            database.CreateTableAsync<Note>().Wait();
            database.CreateTableAsync<Category>().Wait();
        }
        /// <summary>
        /// Get all notes.
        /// </summary>
        /// <returns></returns>
        public Task<List<Note>> GetAllNotesAsync()
        {
            return database.Table<Note>().ToListAsync();
        }
        
        /// <summary>
        /// Get notes from some category.
        /// </summary>
        /// <param name="folderID"></param>
        /// <returns></returns>
        public Task<List<Note>> GetNotesAsync(int folderID)
        {
            return database.Table<Note>().Where(d => d.CategoryID == folderID).ToListAsync();
        }
        /// <summary>
        /// Get all folders.
        /// </summary>
        /// <returns></returns>
        public Task<List<Category>> GetCategoriesAsync()
        {
            return database.Table<Category>().ToListAsync();
        }
        /// <summary>
        /// Get a specific note.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<Note> GetNoteAsync(int id)
        {
            return database.Table<Note>()
                            .Where(i => i.ID == id)
                            .FirstOrDefaultAsync();
        }
        /// <summary>
        /// Get a specific folder.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<Category> GetCategoryAsync(int id)
        {
            return database.Table<Category>()
                            .Where(i => i.ID == id)
                            .FirstOrDefaultAsync();
        }
        /// <summary>
        /// Updates an existing note or save a new one.
        /// </summary>
        /// <param name="note"></param>
        /// <returns></returns>
        public Task<int> SaveNoteAsync(Note note)
        {
            if (note.ID != 0)
            {
                return database.UpdateAsync(note);
            }
            else
            {
                return database.InsertAsync(note);
            }
        }
        /// <summary>
        /// Updates an existing folder or save a new one.
        /// </summary>
        /// <param name="note"></param>
        /// <returns></returns>
        public Task<int> SaveCategoryAsync(Category folder)
        {
            if (folder.ID != 0)
            {
                return database.UpdateAsync(folder);
            }
            else
            {
                return database.InsertAsync(folder);
            }
        }
        /// <summary>
        /// Deletes a note.
        /// </summary>
        /// <param name="note"></param>
        /// <returns></returns>
        public Task<int> DeleteNoteAsync(Note note)
        {
            return database.DeleteAsync(note);
        }
        /// <summary>
        /// Deletes a folder.
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public Task<int> DeleteCategoryAsync(Category folder)
        {
            return database.DeleteAsync(folder);
        }
    }
}