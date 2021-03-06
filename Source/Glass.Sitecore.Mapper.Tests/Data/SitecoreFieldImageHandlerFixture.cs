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
using NUnit.Framework;
using Glass.Sitecore.Mapper.Data;
using Sitecore.Data;
using Sitecore.Data.Items;
using Glass.Sitecore.Mapper.Configuration;
using Glass.Sitecore.Mapper.Configuration.Attributes;
using Glass.Sitecore.Mapper.FieldTypes;
using Sitecore.Data.Fields;
using Sitecore.SecurityModel;
using Sitecore.Links;

namespace Glass.Sitecore.Mapper.Tests.Data
{
    [TestFixture]
    public class SitecoreFieldImageHandlerFixture
    {
        SitecoreFieldImageHandler _handler;
        Database _db;
        Guid _itemId;

        [SetUp]
        public void Setup()
        {
            _db = global::Sitecore.Configuration.Factory.GetDatabase("master");
            _itemId = new Guid("{BD193B3A-D3CA-49B4-BF7A-2A61ED77F19D}");
            _handler = new SitecoreFieldImageHandler();
        }

        #region GetValue

        [Test]
        public void GetValue_ReturnsValidImage()
        {
            //Assign
            Item item = _db.GetItem(new ID(_itemId));
            SitecoreProperty property = new SitecoreProperty()
                {
                    Attribute = new SitecoreFieldAttribute(),
                    Property = new FakePropertyInfo(typeof(Image), "Image")
                };

            _handler.ConfigureDataHandler(property);

            //Act
            var result = _handler.GetValue(item,
                 null) as Image;

            //Assert

            Assert.IsNotNull(result);
            Assert.IsFalse(result.Alt.IsNullOrEmpty());


            Assert.AreEqual(540, result.Height);
            Assert.AreEqual(50, result.HSpace);
            Assert.IsFalse(result.Src.IsNullOrEmpty());
            Assert.AreEqual(60, result.VSpace);
            Assert.AreEqual(720, result.Width);

            // Unsure how to test below
            // result.Border 
            // result.Class
            // result.MediaItem

        }

        #endregion

        #region  SetValue

        [Test]
        public void SetValue_SaveDataCorrectly()
        {
            //Assign
            // /sitecore/media library/Files/DSC01034
            ID newMediaId = new ID(new Guid("{210FAF49-5AA4-471E-BBFE-B4D3ACB6ADC0}"));
            SitecoreProperty property = new SitecoreProperty()
            {
                Attribute = new SitecoreFieldAttribute(),
                Property = new FakePropertyInfo(typeof(Image), "Image")
            };

            _handler.ConfigureDataHandler(property);


            Item item = _db.GetItem(new ID(_itemId));

            Image img = new Image()
            {
                Alt = "New alt",
                Border = "New Border",
                Class = "New Class",
                Height = 70,
                HSpace = 90,
                MediaId = newMediaId.Guid,
                VSpace = 200,
                Width = 567

            };

            using (new SecurityDisabler())
            {
                item.Editing.BeginEdit();
                //Act
                _handler.SetValue( item, img, null);


                //Assert
                ImageField field = new ImageField(item.Fields["Image"]);

                Assert.AreEqual(img.Alt, field.Alt);
                Assert.AreEqual(img.Border, field.Border);
                Assert.AreEqual(img.Class, field.Class);
                Assert.AreEqual(img.Height.ToString(), field.Height);
                Assert.AreEqual(img.HSpace.ToString(), field.HSpace);
                Assert.AreEqual(img.MediaId, field.MediaItem.ID.Guid);
                Assert.AreEqual(img.VSpace.ToString(), field.VSpace);
                Assert.AreEqual(img.Width.ToString(), field.Width);

                item.Editing.CancelEdit();
            }



        }

        [Test]
        public void SetValue_EmptyGuid()
        {
            SitecoreProperty property = new SitecoreProperty()
            {
                Attribute = new SitecoreFieldAttribute(),
                Property = new FakePropertyInfo(typeof(Image), "Image")
            };

            _handler.ConfigureDataHandler(property);


            Item item = _db.GetItem(new ID(_itemId));

            Image img = new Image()
            {
                Alt = "New alt",
                Border = "New Border",
                Class = "New Class",
                Height = 70,
                HSpace = 90,
                MediaId = Guid.Empty,
                VSpace = 200,
                Width = 567

            };

            using (new SecurityDisabler())
            {
                item.Editing.BeginEdit();
                //Act
                _handler.SetValue( item, img, null);


                //Assert
                ImageField field = new ImageField(item.Fields["Image"]);

                Assert.AreEqual(img.Alt, field.Alt);
                Assert.AreEqual(img.Border, field.Border);
                Assert.AreEqual(img.Class, field.Class);
                Assert.AreEqual(img.Height.ToString(), field.Height);
                Assert.AreEqual(img.HSpace.ToString(), field.HSpace);
                Assert.AreEqual(null, field.MediaItem);
                Assert.AreEqual(img.VSpace.ToString(), field.VSpace);
                Assert.AreEqual(img.Width.ToString(), field.Width);

                item.Editing.CancelEdit();
            }
        }


        [Test]
        public void SetValue_HandlesNull()
        {
            //Assign
            // /sitecore/media library/Files/DSC01034
            ID newMediaId = new ID(new Guid("{210FAF49-5AA4-471E-BBFE-B4D3ACB6ADC0}"));
            SitecoreProperty property = new SitecoreProperty()
            {
                Attribute = new SitecoreFieldAttribute(),
                Property = new FakePropertyInfo(typeof(Image), "Image")
            };

            _handler.ConfigureDataHandler(property);


            Item item = _db.GetItem(new ID(_itemId));

            Image img = null;

            using (new SecurityDisabler())
            {
                item.Editing.BeginEdit();
                //Act
                _handler.SetValue(item, img, null);


                //Assert
                Assert.AreEqual("", item.Fields["Image"].Value);

                item.Editing.CancelEdit();
            }



        }
        #endregion
    }
}
