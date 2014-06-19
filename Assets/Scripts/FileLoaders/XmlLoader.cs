﻿using UnityEngine;
using System.Collections;
using System.Xml;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;

public abstract class XmlLoader
{
	
	public LinkedList<T> loadObjectsFromFile<T> (string filePath, string tag)  
		where T : LoadableFromXml,  new()
			
	{
		LinkedList<T> objectList;

		XmlDocument xmlDoc = Tools.getXmlDocument(filePath);

		XmlNodeList objectNodeLists = xmlDoc.GetElementsByTagName(tag);
		
		
		objectList = loadObjects <T>(objectNodeLists);

		return objectList;

	}
	
  public abstract LinkedList<T> loadObjects<T> (XmlNodeList objectNodeLists)
		where T : LoadableFromXml, new();
}

public class XmlLoaderImpl : XmlLoader
{    
  public override LinkedList<T> loadObjects<T> (XmlNodeList objectNodeLists)
      //where T : LoadableFromXml, new()
  {
      LinkedList<T> objectList = new LinkedList<T>();
      T t = new T();

      foreach (XmlNode nodes in objectNodeLists)
      {
        foreach (XmlNode node in nodes)
        {
          if (node.Name == "ATProp")
          {
              t.initFromLoad<T,XmlLoaderImpl>(node, this);
              objectList.AddLast(t);
          }
        }
      }

      if (objectList.Count == 0)
        return null;
      return objectList;
  }
}


