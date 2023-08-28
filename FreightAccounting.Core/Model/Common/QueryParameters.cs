namespace FreightAccounting.Core.Model.Common;

public class QueryParameters
{
    /// <summary>
    /// ماکسیمم سایز 100 تا رکورد هست تا کاربر نتونه پدر دیتابیس رو در بیاره
    /// </summary>
    const int _maxSize = 100;
    private int _size = 50;

    public int Page { get; set; } = 1;

    public int Size
    {
        get
        {
            return _size;
        }
        set
        {
            _size = Math.Min(value, _maxSize);
        }
    }

}
