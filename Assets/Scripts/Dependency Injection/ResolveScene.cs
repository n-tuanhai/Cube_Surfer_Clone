using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;

/// <summary>
/// Resolves dependencies for the entire scene.
/// </summary>
public class ResolveScene : MonoBehaviour
{
    void Awake()
    {
        var dependencyResolver = new DependencyResolver();
        dependencyResolver.ResolveScene();
    }
}