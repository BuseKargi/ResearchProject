using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.OleDb;
using System.Data;
using System.Data.Entity.Validation;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Budget_Project.Models;
using Budget_Project.Filters;
using Budget_Project.Models.ExcelModel;

namespace Budget_Project.Controllers
{
    [Auth]
    public class TransactionController : Controller
    {
        MyBudgetEntities db = new MyBudgetEntities();

        // GET: Transaction
        public ActionResult Index()
        {
            var currentUser = CurrentSession.User.Id;
            var transaction = db.Transaction.Where(x => x.UserId == currentUser).ToList();
            var categoryList = db.Category.Where(x => x.UserId == currentUser).ToList();
            var accountList = db.Account.Where(x => x.UserId == currentUser).ToList();
            if (categoryList.Count <= 0 || accountList.Count <= 0)
            {
                ViewBag.IsDownloadExcel = false;
            }

            ViewBag.IsDownloadExcel = true;
            return View(transaction);
        }
        
        public ActionResult Create()
        {
            var currentUser = CurrentSession.User.Id;
            ViewBag.AccountId = new SelectList(db.Account.Where(x => x.UserId == currentUser).ToList(), "Id", "Name");
            ViewBag.CategoryId = new SelectList(db.Category.Where(x => x.ParentId == 0 && x.IsActive == true).ToList(),
                "Id", "Name");

            return View();
        }

        [HttpPost]
        public ActionResult Create(Transaction model)
        {
            var currentUser = CurrentSession.User.Id;
            ViewBag.AccountId =
                new SelectList(db.Transaction.Where(x => x.UserId == currentUser).ToList(), "Id", "Name");
            ViewBag.CategoryId =
                new SelectList(db.Category.Where(x => x.Id == model.CategoryId && x.IsActive == true).ToList(), "Id",
                    "Name");

            model.AccountId = model.AccountId == null ? null : model.AccountId;
            model.CategoryId = model.CategoryId == null ? null : model.CategoryId;
            model.UserId = currentUser;
            model.CreatedDate = DateTime.Now;
            model.ModifiedDate = DateTime.Now;
            db.Transaction.Add(model);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int? id)
        {
            Transaction transaction = db.Transaction.Find(id);

            var categories = db.Category.ToList();
            var accounts = db.Account.ToList();
            var currentCategory = categories.SingleOrDefault(x => x.Id == transaction.CategoryId);
            var currentAccount = accounts.SingleOrDefault(x => x.Id == transaction.AccountId);

            ViewBag.CategoryId = new SelectList(categories, "Id", "Name", currentCategory.Id);
            ViewBag.AccountId = new SelectList(accounts, "Id", "Name", currentAccount.Id);

            return View(transaction);
        }

        [HttpPost]
        public ActionResult Edit(Transaction model)
        {
            Transaction data = db.Transaction.SingleOrDefault(x => x.Id == model.Id);
            data.CategoryId = model.CategoryId;
            data.AccountId = model.AccountId;
            data.Name = model.Name;
            data.Description = model.Description;
            data.Amount = model.Amount;
            data.ModifiedDate = DateTime.Now;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            if (id != null)
            {
                Transaction data = db.Transaction.FirstOrDefault(x => x.Id == id);
                if (data != null)
                {
                    db.Transaction.Remove(data);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            return RedirectToAction("Index");
        }

        public ActionResult ExportExcel()
        {
            var filePath = "example.xlsx";

            using (var spreadsheetDocument =
                   SpreadsheetDocument.Create(filePath, SpreadsheetDocumentType.Workbook))
            {
                var workbookPart = spreadsheetDocument.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                var sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild(new Sheets());

                var exampleWorksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                exampleWorksheetPart.Worksheet = new Worksheet(new SheetData());

                var categoryWorksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                categoryWorksheetPart.Worksheet = new Worksheet(new SheetData());

                var accountWorksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                accountWorksheetPart.Worksheet = new Worksheet(new SheetData());

                var sheet = new Sheet()
                {
                    Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(exampleWorksheetPart),
                    SheetId = 1,
                    Name = "Example"
                };

                var categorySheet = new Sheet()
                {
                    Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(categoryWorksheetPart),
                    SheetId = 2,
                    Name = "Category"
                };

                var accountSheet = new Sheet()
                {
                    Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(accountWorksheetPart),
                    SheetId = 3,
                    Name = "Account"
                };

                var exampleSheetData = exampleWorksheetPart.Worksheet.GetFirstChild<SheetData>();
                var categorySheetData = categoryWorksheetPart.Worksheet.GetFirstChild<SheetData>();
                var accountSheetData = accountWorksheetPart.Worksheet.GetFirstChild<SheetData>();

                var exampleHeaderRow = new Row();
                var categoryHeaderRow = new Row();
                var accountHeaderRow = new Row();

                exampleHeaderRow.Append(
                    new Cell() { DataType = CellValues.String, CellValue = new CellValue("Title") },
                    new Cell() { DataType = CellValues.String, CellValue = new CellValue("Description") },
                    new Cell() { DataType = CellValues.String, CellValue = new CellValue("Amount") },
                    new Cell() { DataType = CellValues.String, CellValue = new CellValue("CategoryId") },
                    new Cell() { DataType = CellValues.String, CellValue = new CellValue("AccountId") }
                );

                categoryHeaderRow.Append(
                    new Cell() { DataType = CellValues.String, CellValue = new CellValue("Id") },
                    new Cell() { DataType = CellValues.String, CellValue = new CellValue("Name") }
                );

                accountHeaderRow.Append(
                    new Cell() { DataType = CellValues.String, CellValue = new CellValue("Id") },
                    new Cell() { DataType = CellValues.String, CellValue = new CellValue("Name") }
                );

                exampleSheetData.AppendChild(exampleHeaderRow);
                categorySheetData.AppendChild(categoryHeaderRow);
                accountSheetData.AppendChild(accountHeaderRow);

                var currentUser = CurrentSession.User.Id;

                var categoryList = db.Category.Where(x => x.UserId == currentUser).ToList();
                var categoryData = new List<ExcelCategory>();

                if (categoryList.Count > 0)
                {
                    foreach (var data in categoryList)
                    {
                        var newData = new ExcelCategory()
                        {
                            Id = data.Id,
                            Name = data.Name,
                        };
                        categoryData.Add(newData);
                    }
                }

                var accountList = db.Account.Where(x => x.UserId == currentUser).ToList();
                var accountData = new List<ExcelAccount>();

                if (accountList.Count > 0)
                {
                    foreach (var data in accountList)
                    {
                        var newData = new ExcelAccount()
                        {
                            Id = data.Id,
                            Name = data.Name,
                        };
                        accountData.Add(newData);
                    }
                }

                foreach (var category in categoryData)
                {
                    var newRow = new Row();

                    var cell1 = new Cell()
                    {
                        DataType = CellValues.Number,
                        CellValue = new CellValue(category.Id)
                    };
                    newRow.AppendChild(cell1);

                    var cell2 = new Cell()
                    {
                        DataType = CellValues.String,
                        CellValue = new CellValue(category.Name)
                    };
                    newRow.AppendChild(cell2);

                    categorySheetData.AppendChild(newRow);
                }

                foreach (var account in accountData)
                {
                    var newRow = new Row();

                    var cell1 = new Cell()
                    {
                        DataType = CellValues.Number,
                        CellValue = new CellValue(account.Id)
                    };
                    newRow.AppendChild(cell1);

                    var cell2 = new Cell()
                    {
                        DataType = CellValues.String,
                        CellValue = new CellValue(account.Name)
                    };
                    newRow.AppendChild(cell2);

                    accountSheetData.AppendChild(newRow);
                }

                sheets.Append(sheet);
                sheets.Append(categorySheet);
                sheets.Append(accountSheet);

                workbookPart.Workbook.Save();
            }

            var fileBytes = System.IO.File.ReadAllBytes("example.xlsx");
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "example.xlsx");
        }

        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength <= 0)
                return Json(new { success = false, message = "Dosya yüklenemedi." });
            try
            {
                var fileName = System.IO.Path.GetFileName(file.FileName);
                var path = System.IO.Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
                file.SaveAs(path);
                var excelDataList = ReadExcelData(path);

                for (var index = 0; index < excelDataList.Count; index++)
                {
                    var result = excelDataList[index];
                    if (string.IsNullOrEmpty(result.Title) || string.IsNullOrEmpty(result.Description) ||
                        result.CategoryId <= 0 || result.AccountId <= 0 || result.Amount <= 0)
                    {
                        excelDataList.Remove(result);
                    }
                }

                var errorList = new List<ErrorList>();

                foreach (var data in excelDataList)
                {
                    var checkCategoryId = db.Category.SingleOrDefault(x => x.Id == data.CategoryId);
                    if (checkCategoryId == null)
                    {
                        var error = new ErrorList()
                        {
                            Title = data.Title,
                            Type = "Kategori Id Yanlış"
                        };
                        errorList.Add(error);
                        continue;
                    }

                    var checkAccountId = db.Account.SingleOrDefault(x => x.Id == data.AccountId);
                    if (checkAccountId == null)
                    {
                        var error = new ErrorList()
                        {
                            Title = data.Title,
                            Type = "Hesap Id Yanlış"
                        };
                        errorList.Add(error);
                        continue;
                    }

                    var newData = new Transaction()
                    {
                        Name = data.Title,
                        Description = data.Description,
                        Amount = data.Amount,
                        CategoryId = data.CategoryId,
                        AccountId = data.AccountId,
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now,
                        UserId = CurrentSession.User.Id,
                    };
                    db.Transaction.Add(newData);
                    db.SaveChanges();
                }

                return Json(new { success = true, message = "Kayıtlar Başarıyla Eklendi", errors = errorList });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Dosya yüklenirken bir hata oluştu: " + ex.Message });
            }
        }

        public List<ExcelTransaction> ReadExcelData(string filePath)
        {
            var excelDataList = new List<ExcelTransaction>();

            using (var spreadsheetDocument = SpreadsheetDocument.Open(filePath, false))
            {
                var workbookPart = spreadsheetDocument.WorkbookPart;
                var sheet = workbookPart.Workbook.Descendants<Sheet>().FirstOrDefault(s => s.Name == "Example");

                if (sheet != null)
                {
                    var worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheet.Id);
                    var sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();

                    foreach (var row in
                             sheetData.Elements<Row>().Skip(1))
                    {
                        var excelData = new ExcelTransaction();
                        var colIndex = 0;

                        foreach (var cell in row.Elements<Cell>())
                        {
                            var cellValue = GetCellValue(cell, workbookPart);

                            switch (colIndex)
                            {
                                case 0:
                                    excelData.Title = cellValue;
                                    break;
                                case 1:
                                    excelData.Description = cellValue;
                                    break;
                                case 2:
                                    excelData.Amount = decimal.Parse(cellValue);
                                    break;
                                case 3:
                                    excelData.CategoryId = int.Parse(cellValue);
                                    break;
                                case 4:
                                    excelData.AccountId = int.Parse(cellValue);
                                    break;
                            }

                            colIndex++;
                        }

                        excelDataList.Add(excelData);
                    }
                }
            }

            return excelDataList;
        }

        private static string GetCellValue(Cell cell, WorkbookPart workbookPart)
        {
            var value = cell.InnerText;

            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                var ssid = int.Parse(value);
                var stringTablePart = workbookPart.SharedStringTablePart;
                if (stringTablePart != null)
                {
                    value = stringTablePart.SharedStringTable.ElementAt(ssid).InnerText;
                }
            }

            return value;
        }
    }
}