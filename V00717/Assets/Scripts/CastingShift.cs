using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CastingShift : MonoBehaviour
{
    private GameObject character;
    private GameObject roleDropdown;
    private GameObject activeToggle;
    private GameObject beginTimeDropdown;
    private GameObject endTimeDropdown;
    private GameObject castNameTMP_Text;

    void Awake()
    {
        roleDropdown = this.gameObject.GetComponentInChildren<RoleDropDown>().gameObject;
        activeToggle = this.gameObject.GetComponentInChildren<ActiveToggle>().gameObject;
        beginTimeDropdown = this.gameObject.GetComponentInChildren<BeginTimeDropDown>().gameObject;
        endTimeDropdown = this.gameObject.GetComponentInChildren<EndTimeDropDown>().gameObject;
        castNameTMP_Text = this.gameObject.GetComponentInChildren<CastName>().gameObject;
    }
    public void BindCharacter(GameObject c)
    {
        character = c;
        string name = c.GetComponent<CharacterModel>().name;
        castNameTMP_Text.GetComponent<TMP_Text>().SetText(name);
    }
    public GameObject GetRoleDropDown()
    {
        return roleDropdown;
    }
    public string GetRoleDropDownValue()
    {
        Dropdown dropDown = roleDropdown.GetComponent<Dropdown>();
        return dropDown.options[dropDown.value].text;
    }
    public GameObject GetActiveToggle()
    {
        return activeToggle;
    }
    public bool GetActiveIsOn()
    {
        return activeToggle.GetComponent<Toggle>().isOn;
    }
    public GameObject GetBeginTimeDropDown()
    {
        return beginTimeDropdown;
    }
    public string GetBeginTime()
    {
        Dropdown dropDown = beginTimeDropdown.GetComponent<Dropdown>();
        return dropDown.options[dropDown.value].text;
    }
    public GameObject GetEndTimeDropDown()
    {
        return endTimeDropdown;
    }
    public string GetEndTime()
    {
        Dropdown dropDown = endTimeDropdown.GetComponent<Dropdown>();
        return dropDown.options[dropDown.value].text;
    }
    public GameObject GetCastNameTextMeshProUGUI()
    {
        return castNameTMP_Text;
    }
    public string GetCastName()
    {
        return castNameTMP_Text.GetComponent<TMP_Text>().text;
    }
}
