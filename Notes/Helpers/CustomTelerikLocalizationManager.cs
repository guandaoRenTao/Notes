using System;
using System.Collections.Generic;
using System.Text;
using Telerik.XamarinForms.Common;
namespace Notes.Helpers
{
    public class CustomTelerikLocalizationManager : TelerikLocalizationManager
    {
        public override string GetString(string key)
        {
            if (key == "RichTextEditor_AddHyperlink")
            {
                return "Ссылка";
            }

            if (key == "RichTextEditor_AddImage")
            {
                return "Изображение";
            }

            if (key == "RichTextEditor_Subscript")
            {
                return "Нижний регистр";
            }
            
            if (key == "RichTextEditor_Superscript")
            {
                return "Верхний регистр";
            }
            
            if (key == "RichTextEditor_Strike")
            {
                return "Зачеркнуто";
            }
            if (key == "RichTextEditor_HyperlinkHeaderText")
            {
                return "Ссылка";
            }
            if (key == "RichTextEditor_HyperlinkCancelButtonText")
            {
                return "Отмена";
            }
            
            if (key == "RichTextEditor_Copy")
            {
                return "Копировать";
            }
            if (key == "RichTextEditor_Cut")
            {
                return "Вырезать";
            }
            
            if (key == "RichTextEditor_PastePlainText")
            {
                return "Вставить как текст";
            }
            if (key == "RichTextEditor_Paste")
            {
                return "Вставить";
            }
            if (key == "RichTextEditor_SelectAll")
            {
                return "Выбрать все";
            }
            if (key == "RichTextEditor_ImageResizeLabelText")
            {
                return "Размер";
            }
            if (key == "RichTextEditor_ImageCancelButtonText")
            {
                return "Отмена";
            }

            return base.GetString(key);
        }
    }
}
