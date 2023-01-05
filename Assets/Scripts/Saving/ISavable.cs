using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface ISavable
{
    object SaveData();
    void LoadData(object data);
}
