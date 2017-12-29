﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Zony.Lib.Plugin.Attributes;
using Zony.Lib.Plugin.Interfaces;

namespace Zony.Lib.Plugin
{
    public class PluginManager : IPluginManager
    {
        private readonly ConcurrentDictionary<Type, List<Type>> m_pluginContainer;
        private readonly string m_pluginsFolderPath = Environment.CurrentDirectory + @"\Plugins";

        public PluginManager()
        {
            m_pluginContainer = new ConcurrentDictionary<Type, List<Type>>();
        }

        public List<PluginInfoAttribute> GetAllPluginInfos()
        {
            var _result = new List<PluginInfoAttribute>();

            foreach (var _type in m_pluginContainer)
            {
                foreach (var _plugin in _type.Value)
                {
                    var _info = _plugin.GetCustomAttribute<PluginInfoAttribute>();
                    if (_info != null) _result.Add(_info);
                }
            }

            return _result;
        }

        public TInterface GetPlugin<TInterface>() where TInterface : class
        {
            Type _type = typeof(TInterface);

            if (m_pluginContainer.TryGetValue(_type, out List<Type> _plugins))
            {
                return Activator.CreateInstance(_plugins[0]) as TInterface;
            }

            return default(TInterface);
        }

        public List<TInterface> GetPlugins<TInterface>() where TInterface : class
        {
            Type _type = typeof(TInterface);

            List<TInterface> _instances = new List<TInterface>();

            if (!m_pluginContainer.TryGetValue(_type, out List<Type> _plugins)) return null;
            _plugins.ForEach(_plugin => _instances.Add(Activator.CreateInstance(_plugin) as TInterface));
            return _instances;
        }

        public void LoadPlugins()
        {
            LoadPlugins(m_pluginsFolderPath);
        }

        public void LoadPlugins(string dirPath)
        {
            if (!Directory.Exists(dirPath)) return;

            string[] _files = Directory.GetFiles(dirPath, "*.dll");
            foreach (var _file in _files)
            {
                Assembly _asm = Assembly.UnsafeLoadFrom(_file);

                Type[] _types = _asm.GetTypes();
                List<Type> _plugins = new List<Type>();
                Type _interfaceType = null;

                foreach (var _type in _types)
                {
                    if (_type.GetInterface(typeof(IPlugin).Name) != null)
                    {
                        _interfaceType = _type.GetInterfaces().Where(x => x != typeof(IPlugin)).FirstOrDefault();

                        _plugins.Add(_type);
                    }
                }

                if (_plugins.Count != 0)
                {
                    m_pluginContainer.GetOrAdd(_interfaceType, _plugins);
                }
            }
        }
    }
}