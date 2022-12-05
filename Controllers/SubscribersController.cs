using System.Data;
using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using SubcriptionManagement.Models;

namespace SubcriptionManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscribersController : ControllerBase
    {
        private readonly SubcriptionDbContext _context;

        public SubscribersController(SubcriptionDbContext context)
        {
            _context = context;
        }

        // POST: api/Subscribers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Subscriber>> PostSubscriber(IFormFile file)
        {
            List<Subscriber> subscribers = new List<Subscriber>();
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            DataRow dataRow;
            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                stream.Position = 0;
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet();
                    DataRowCollection dt = result.Tables[0].Rows;
                    dataRow = dt[0];
                    foreach (var item in dt)
                    {
                        if (int.TryParse(((DataRow)item).ItemArray[0].ToString(), out int value))
                        {


                            Guid subscriberId = Guid.NewGuid();
                            Subscriber subscriber = new Subscriber
                            {
                                SubscriberId = subscriberId,
                                Name = ((DataRow)item).ItemArray[1].ToString(),
                                Gender = ((DataRow)item).ItemArray[2].ToString().ToLower() == "male" ? true : false,
                                Age = Convert.ToInt16(((DataRow)item).ItemArray[3]),
                                SubscriptionType = (SubscriptionType)Enum.ToObject(typeof(SubscriptionType), Convert.ToInt16(((DataRow)item).ItemArray[9])),
                                Price = Convert.ToInt16(((DataRow)item).ItemArray[10]),
                                StartDate = Convert.ToDateTime(((DataRow)item).ItemArray[11]),
                                EndDate = Convert.ToDateTime(((DataRow)item).ItemArray[12])
                            };
                            BindingSubscriberChoices(dataRow, item, subscriberId, subscriber);
                            subscribers.Add(subscriber);
                        }
                    }
                }
            }
            _context.Subscribers.AddRange(subscribers);
            await _context.SaveChangesAsync();

            //Report Generation
            AbstractDataExportBridge abstractDataExportBridge = new AbstractDataExportBridge();
            IWorkbook _workbook = new XSSFWorkbook(); //Creating New Excel object
            var headerStyle = _workbook.CreateCellStyle(); //Formatting
            var headerFont = _workbook.CreateFont();
            headerFont.IsBold = true;
            headerStyle.SetFont(headerFont);

            SetReport(subscribers, abstractDataExportBridge, _workbook, headerStyle);
            SetReportDetails(subscribers, abstractDataExportBridge, _workbook, headerStyle);
            SetReportGenderWise(subscribers, abstractDataExportBridge, _workbook, headerStyle);

            using (var memoryStream = new MemoryStream()) //creating memoryStream
            {
                _workbook.Write(memoryStream, false);

                return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Report.xlsx");
            }
        }

        private static void SetReportGenderWise(List<Subscriber> subscribers, AbstractDataExportBridge abstractDataExportBridge, IWorkbook _workbook, ICellStyle headerStyle)
        {
            ISheet _sheetReportDetails = _workbook.CreateSheet("ReportGenderWise"); //Creating New Excel Sheet object
            List<string> _headers = new List<string>(), _type = new List<string>();

            List<GenderSpecificDetail> genderSpecificDetails = subscribers.GroupBy(x => x.Gender).AsEnumerable()
                          .Select(x => new GenderSpecificDetail
                          {
                              Gender = Convert.ToBoolean(x.Key),
                              TotalPrice = x.Sum(p => p.Price)
                          }).ToList();
            abstractDataExportBridge.WriteGenderWise(genderSpecificDetails, _sheetReportDetails, out _headers, out _type);
            ////Header
            var header = _sheetReportDetails.CreateRow(0);
            SetHeader(headerStyle, _sheetReportDetails, _headers, header);
        }
        private static void SetReportDetails(List<Subscriber> subscribers, AbstractDataExportBridge abstractDataExportBridge, IWorkbook _workbook, ICellStyle headerStyle)
        {
            ISheet _sheetReportDetails = _workbook.CreateSheet("ReportDetails"); //Creating New Excel Sheet object
            List<string> _headers = new List<string>(), _type = new List<string>();

            List<SelectiveSubscriber> selectedSubcribers = subscribers.Where(x => x.SubscriptionType == SubscriptionType.Twelve).AsEnumerable()
                          .Select(o => new SelectiveSubscriber
                          {
                              Name = o.Name,
                              Age = o.Age,
                              Gender = o.Gender
                          }).ToList();

            abstractDataExportBridge.WriteData(selectedSubcribers, _sheetReportDetails, out _headers, out _type);
            //Header
            var header = _sheetReportDetails.CreateRow(0);
            SetHeader(headerStyle, _sheetReportDetails, _headers, header);
        }

        private static void SetReport(List<Subscriber> subscribers, AbstractDataExportBridge abstractDataExportBridge, IWorkbook _workbook, ICellStyle headerStyle)
        {
            ISheet _sheetReport = _workbook.CreateSheet("Report");
            List<Report> reports = new List<Report>
            {
                new Report
                {
                    TotalMale = subscribers.Count(x => x.Gender),
                    TotalFemale = subscribers.Count(x => !x.Gender),
                    TotalRecords = subscribers.Count,
                    CurrentYearSubscriber = subscribers.Count(x => x.SubscriptionType == SubscriptionType.Twelve)
                }
            };
            List<string> _headersReport = new List<string>(), _typeReport = new List<string>();
            abstractDataExportBridge.WriteSingle(reports, _sheetReport, out _headersReport, out _typeReport);

            var headerReport = _sheetReport.CreateRow(0);
            SetHeader(headerStyle, _sheetReport, _headersReport, headerReport);
        }

        private static void SetHeader(ICellStyle headerStyle, ISheet _sheet, List<string> _headers, IRow header)
        {
            for (var i = 0; i < _headers.Count; i++)
            {
                var cell = header.CreateCell(i);
                cell.SetCellValue(_headers[i]);
                cell.CellStyle = headerStyle;
            }

            for (var i = 0; i < _headers.Count; i++)
            {
                _sheet.AutoSizeColumn(i);
            }
        }

        //https://stackoverflow.com/questions/2206279/exporting-the-values-in-list-to-excel

        private static void BindingSubscriberChoices(DataRow dataRow, object item, Guid subscriberId, Subscriber subscriber)
        {
            Guid subscriberChoiceId = new Guid();
            //Horror
            subscriber.SubscriberChoices.Add(new SubscriberChoice
            {
                SubscriberChoiceId = subscriberChoiceId,
                SubscriberId = subscriberId,
                Choice = dataRow.ItemArray[4].ToString(),
                ChoiceCount = Convert.ToInt16(((DataRow)item).ItemArray[4])
            });
            //Comedy
            subscriber.SubscriberChoices.Add(new SubscriberChoice
            {
                SubscriberChoiceId = subscriberChoiceId,
                SubscriberId = subscriberId,
                Choice = dataRow.ItemArray[5].ToString(),
                ChoiceCount = Convert.ToInt16(((DataRow)item).ItemArray[5])
            });
            //Series
            subscriber.SubscriberChoices.Add(new SubscriberChoice
            {
                SubscriberChoiceId = subscriberChoiceId,
                SubscriberId = subscriberId,
                Choice = dataRow.ItemArray[6].ToString(),
                ChoiceCount = Convert.ToInt16(((DataRow)item).ItemArray[6])
            });
            //Sci-Fi	
            subscriber.SubscriberChoices.Add(new SubscriberChoice
            {
                SubscriberChoiceId = subscriberChoiceId,
                SubscriberId = subscriberId,
                Choice = dataRow.ItemArray[7].ToString(),
                ChoiceCount = Convert.ToInt16(((DataRow)item).ItemArray[7])
            });
            //Animation
            subscriber.SubscriberChoices.Add(new SubscriberChoice
            {
                SubscriberChoiceId = subscriberChoiceId,
                SubscriberId = subscriberId,
                Choice = dataRow.ItemArray[8].ToString(),
                ChoiceCount = Convert.ToInt16(((DataRow)item).ItemArray[8])
            });
        }
    }
}
