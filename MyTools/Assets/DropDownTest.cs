using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace YouLe
{
    ///  <summary>
    ///
    ///  </summary>
    public class DropDownTest : MonoBehaviour
    {
        private List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        private void Start()
        {
            for (int i = 0; i < 10; i++)
            {
                options.Add(new Dropdown.OptionData(i.ToString()));
            }
            GetComponent<Dropdown>().options = options;
        }
    }
}