using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Constructor must not have any parameters
/// </summary>
public abstract class Singleton<T> where T: class, new() 
{
    protected static T instance = null;
    public static T Instance { get => instance ?? (instance = new T()); }
    //public static T Instance { get => instance ?? (instance = Activator.CreateInstance(typeof(T)) as T); }
}
