using System.ComponentModel;
using System.Data;
using System.Text.RegularExpressions;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;

namespace SubcriptionManagement.Models
{
    public class AbstractDataExportBridge 
    {

        public void WriteData(List<SelectiveSubscriber> exportData, ISheet _sheet, out List<string> _headers, out List<string> _type)
        {
            _headers = new List<string>();
            _type = new List<string>();

            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(SelectiveSubscriber));

            DataTable table = BindProperties(_headers, _type, properties);

            foreach (var item in exportData)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }

            BindDataRowWise(_sheet, _type, table);
        }

        public void WriteSingle(List<Report> exportData, ISheet _sheet, out List<string> _headers, out List<string> _type)
        {
            _headers = new List<string>();
            _type = new List<string>();

            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(Report));
            DataTable table = BindProperties(_headers, _type, properties);

            foreach (var item in exportData)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }

            BindDataRowWise(_sheet, _type, table);
        }

        public void WriteGenderWise(List<GenderSpecificDetail> exportData, ISheet _sheet, out List<string> _headers, out List<string> _type)
        {
            _headers = new List<string>();
            _type = new List<string>();

            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(GenderSpecificDetail));
            DataTable table = BindProperties(_headers, _type, properties);

            foreach (var item in exportData)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }

            BindDataRowWise(_sheet, _type, table);
        }

        private static DataTable BindProperties(List<string> _headers, List<string> _type, PropertyDescriptorCollection properties)
        {
            DataTable table = new DataTable();

            foreach (PropertyDescriptor prop in properties)
            {
                var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                _type.Add(type.Name);
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ??
                                  prop.PropertyType);
                string name = Regex.Replace(prop.Name, "([A-Z])", " $1").Trim(); //space separated 
                                                                                 //name by caps for header
                _headers.Add(name);
            }

            return table;
        }

        private static void BindDataRowWise(ISheet _sheet, List<string> _type, DataTable table)
        {
            IRow sheetRow = null;

            for (int i = 0; i < table.Rows.Count; i++)
            {
                sheetRow = _sheet.CreateRow(i + 1);
                for (int j = 0; j < table.Columns.Count; j++)
                {
                    ICell Row1 = sheetRow.CreateCell(j);

                    string type = _type[j].ToLower();
                    var currentCellValue = table.Rows[i][j];

                    if (currentCellValue != null &&
                        !string.IsNullOrEmpty(Convert.ToString(currentCellValue)))
                    {
                        if (type == "string")
                        {
                            Row1.SetCellValue(Convert.ToString(currentCellValue));
                        }
                        else if (type == "int32")
                        {
                            Row1.SetCellValue(Convert.ToInt32(currentCellValue));
                        }
                        else if (type == "double")
                        {
                            Row1.SetCellValue(Convert.ToDouble(currentCellValue));
                        }
                        else if (type == "boolean")
                        {
                            Row1.SetCellValue(Convert.ToBoolean(currentCellValue) ? Gender.Male.ToString() : Gender.Female.ToString());
                        }
                    }
                    else
                    {
                        Row1.SetCellValue(string.Empty);
                    }
                }
            }
        }
    }
}
