using System;

public class TopupModel
{
 public DataObject data{get;set;}
 public string uri{get;set;}
}

public class DataObject
{
    public Dictionary<string,List<string>> FirstBlock{get;set;}

    public List<string> SecondBlock{get;set;}
}