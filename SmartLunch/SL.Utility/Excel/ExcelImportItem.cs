namespace SL.Utility
{
    public class ExcelHelperItem
    {
        private string _field_name;
        public string FieldName
        {
            set { _field_name = value; }
            get { return _field_name; }
        }

        private string _default_value;
        public string Default_Value
        {
            set { _default_value = value; }
            get { return _default_value; }
        }

        private int _seq;
        public int Seq
        {
            set { _seq = value; }
            get { return _seq; }
        }

        private bool _empty;
        public bool Empty
        {
            set { _empty = value; }
            get { return _empty; }
        }

        private string _type;
        public string Type
        {
            set { _type = value; }
            get { return _type; }
        }

        private string _length;
        public string Length
        {
            set { _length = value; }
            get { return _length; }
        }

        private string _memo;
        public string Memo
        {
            set { _memo = value; }
            get { return _memo; }
        }
    }
}
