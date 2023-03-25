using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CVillager: CData
{
    public EVillagerType type = EVillagerType.Administrator;
    public float meal_amount = 0f;
    public float salary_amount = 0f;
    public float pension_amount = 1f;
    public float hire_price = 0f;
    public int number_of_limit = 0;
    public int size_of_house = 2;
    public int stay_bID = 80;
    public int stay_dates = 0;
    public List<int> buildingIds;
}
