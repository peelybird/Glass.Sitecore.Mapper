﻿/*
   Copyright 2011 Michael Edwards
 
   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Glass.Sitecore.Mapper.Configuration.Attributes;
using Sitecore.Links;
using Glass.Sitecore.Mapper.Configuration;

namespace Glass.Sitecore.Mapper.Data
{
    public class SitecoreInfoHandler : AbstractSitecoreDataHandler
    {
        public SitecoreInfoType InfoType
        {
            get;
            set;
        }

        public UrlOptions UrlOptions
        {
            get;
            set;
        }

        public override bool WillHandle(Glass.Sitecore.Mapper.Configuration.SitecoreProperty property, IEnumerable<AbstractSitecoreDataHandler> datas, Dictionary<Type, SitecoreClassConfig> classes)
        {
            return property.Attribute is SitecoreInfoAttribute;
        }

        public override object GetValue(global::Sitecore.Data.Items.Item item, ISitecoreService service)
        {
            
            switch (InfoType)
            {
                case SitecoreInfoType.ContentPath:
                    return item.Paths.ContentPath;
                case SitecoreInfoType.DisplayName:
                    return item.DisplayName;
                case SitecoreInfoType.FullPath:
                    return item.Paths.FullPath;
                case SitecoreInfoType.Name:
                    return item.Name;
                case SitecoreInfoType.Key:
                    return item.Key;
                case SitecoreInfoType.MediaUrl:
                    global::Sitecore.Data.Items.MediaItem media = new global::Sitecore.Data.Items.MediaItem(item);
                    return global::Sitecore.Resources.Media.MediaManager.GetMediaUrl(media);
                    break;
                case SitecoreInfoType.Path:
                    return item.Paths.Path;
                case SitecoreInfoType.TemplateId:
                    return item.TemplateID.Guid;
                case SitecoreInfoType.TemplateName:
                    return item.TemplateName;
                case SitecoreInfoType.Url:
                    return LinkManager.GetItemUrl(item, UrlOptions);
                case SitecoreInfoType.FullUrl:
                    return LinkManager.GetItemUrl(item, new UrlOptions() { AlwaysIncludeServerUrl = true });
                case SitecoreInfoType.Version:
                    return item.Version.Number;
                case SitecoreInfoType.Language:
                    return item.Language;
                default:
                    throw new NotSupportedException("Value {0} not supported".Formatted(InfoType.ToString()));
            }


        }

        public override void SetValue(global::Sitecore.Data.Items.Item item, object value, ISitecoreService service)
        {
            
            switch (InfoType)
            {
                case SitecoreInfoType.DisplayName:
                    if (value is string || value == null)
                        item[Settings.DisplayNameFieldName] = (value ?? string.Empty).ToString();
                    else
                        throw new NotSupportedException("Can't set DisplayName. Value is not of type System.String");
                    break;
                case SitecoreInfoType.Name:
                    if (value is string || value == null)
                    {
                        //if the name is null or empty nothing should happen
                        if ((value ?? string.Empty).ToString().IsNullOrEmpty()) return;
                        
                        if (item.Name != value.ToString())
                        {
                            item.Name = value.ToString();
                        }
                        
                    }
                    else
                        throw new NotSupportedException("Can't set Name. Value is not of type System.String");
                    break;             
                default:
                    throw new NotSupportedException("You can not save SitecoreInfo {0}".Formatted(InfoType));
            }

        }

        public override bool CanSetValue
        {
            get
            {
                return (InfoType == SitecoreInfoType.DisplayName || InfoType == SitecoreInfoType.Name);
            }
        }

        internal override void ConfigureDataHandler(SitecoreProperty scProperty)
        {
            SitecoreInfoAttribute attr = scProperty.Attribute as SitecoreInfoAttribute;

            InfoType = attr.Type;

                UrlOptions = Utility.CreateUrlOptions(attr.UrlOptions);

            base.ConfigureDataHandler(scProperty);
        }

    }
}
