using System.Collections.Generic;

/**<p>Model classes for use in the model tier encapsulate and provide an API for data.
 * Models send event notifications when work has been performed on the data model.
 * Models are generally highly portable entities.</p>*/
public class UserCountsModel : Singleton<UserCountsModel>
{
    public List<List<string>> uCounts = new List<List<string>>();
    public int total; //Tổng số người login vào game chắn
    public List<int> subTotals = new List<int>(); //subTotals[z] là số người trong zone z
}