using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using TicketingSystem.Models;

namespace TicketingSystem.Web.Helpers
{
	public static class AttachmentManager
	{
		public static Attachment AddAttachment(HttpPostedFileBase file, Ticket ticket)
		{
			var filePath = GetFilePath(file, ticket);
			var directoryPath = new FileInfo(filePath).Directory.FullName;

			Directory.CreateDirectory(directoryPath);
			file.SaveAs(filePath);

			var attachment = new Attachment
			{
				Name = Path.GetFileName(file.FileName),
				Path = filePath
			};

			return attachment;
		}

		public static void RemoveAttachment(Attachment attachment)
		{
			File.Delete(attachment.Path);
		}

		private static string GetFilePath(HttpPostedFileBase file, Ticket ticket)
		{
			var pathPattern = GetUploadPathPattern();
			var filePath = string.Format(pathPattern, AppDomain.CurrentDomain.BaseDirectory,
				ticket.Id.ToString(), Path.GetFileName(file.FileName));

			if (File.Exists(filePath))
			{
				return GetNewFileName(filePath);
			}

			return filePath;
		}

		private static string GetNewFileName(string filePath, int counter = 1)
		{
			var newFileName = string.Format("{0}/{1}_{2}{3}",
				Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath), counter.ToString(), Path.GetExtension(filePath));

			if (File.Exists(newFileName))
			{
				return GetNewFileName(filePath, counter + 1);
			}

			return newFileName;
		}

		private static string GetUploadPathPattern()
		{
			var uploadPathPattern = ConfigurationManager.AppSettings.Get("UploadPathPattern");

			if (string.IsNullOrEmpty(uploadPathPattern))
			{
				throw new ArgumentNullException("Missing upload path pattern");
			}

			return uploadPathPattern;
		}
	}
}