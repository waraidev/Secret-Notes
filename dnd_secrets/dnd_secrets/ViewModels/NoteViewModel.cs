using System;
using System.Collections.Generic;
using System.IO;
using dnd_secrets.Models;

namespace dnd_secrets.ViewModels
{
    public class NoteViewModel
    {
        static List<Note> _notes;
        static IEnumerable<string> _files;

        public static string FolderPath = Path
            .Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData
                )
            );

        public static List<Note> Notes
        {
            get
            {
                _notes = _notes == null ? new List<Note>() : _notes;

                return _notes;
            }
        }
    }
}
