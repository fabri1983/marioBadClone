//AlmostLogical Software - http://www.almostlogical.com - support@almostlogical.com
using System.Collections.Generic;
using UnityEngine;

public class PPGOSetting
{
    private List<PPComponentSetting> componentSettings;

    public PPGOSetting(GameObject gameObj)
    {
        this.GameObj = gameObj;

        CreateComponentSettings();
    }

    public GameObject GameObj { get; set; }

    public bool Expanded { get; set; }

    public bool SaveAll { get; set; }

    public List<PPComponentSetting> ComponentSettings
    {
        get { return componentSettings; }
    }

    public void CreateComponentSettings()
    {
        componentSettings = new List<PPComponentSetting>();

        Component[] components = GameObj.GetComponents(typeof(Component));

        foreach (Component c in components)
        {
            PPComponentSetting setting = new PPComponentSetting(c);

            if (c != null)
            {
                if (PPLocalStorageManager.IsTypeDefaulted(c.GetType()))
                {
                    setting.IsSavingSettings = true;
                }
            }

            componentSettings.Add(setting);
        }
    }

    public void StoreAllSelectedSettings()
    {
        componentSettings.ForEach(setting => setting.StoreSettings());
    }

    //return list of components which settings have been restored
    public List<Component> RestoreAllSelectedSettings()
    {
        List<Component> listOfChangedComponents = new List<Component>();
        Component resultChangedComponent = null;
        foreach (PPComponentSetting setting in componentSettings)
        {
            resultChangedComponent = setting.RestoreSettings();
            if (resultChangedComponent!=null)
            {
                listOfChangedComponents.Add(resultChangedComponent);
            }
        }

        return listOfChangedComponents;
    }
}