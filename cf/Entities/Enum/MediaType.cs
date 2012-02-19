using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cf.Entities.Enum
{
    public enum MediaType : byte
    {
        Unknown = 0, 
        Image = 10, //-- FileName
        Youtube = 22, //-- Embed Html
        Vimeo = 25, //-- Embed Html
        Website = 30, //-- Link
        BlogPost = 33, //-- Link
        Article = 36, //-- Link
        Wrd = 41, //-- Link
        Pdf = 44, //-- Link
        Html = 56, //-- Stored in Content
    }
}
