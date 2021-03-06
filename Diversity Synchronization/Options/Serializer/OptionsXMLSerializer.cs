﻿//#######################################################################
//Diversity Mobile Synchronization
//Project Homepage:  http://www.diversitymobile.net
//Copyright (C) 2011  Tobias Schneider, Lehrstuhl Angewndte Informatik IV, Universität Bayreuth
//
//This program is free software; you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation; either version 2 of the License, or
//(at your option) any later version.
//
//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.
//
//You should have received a copy of the GNU General Public License along
//with this program; if not, write to the Free Software Foundation, Inc.,
//51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
//#######################################################################

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace Diversity_Synchronization.Options.Serializer
{
    /**
     * Implementierung der Optionen als XML-Datei
     */
    public class OptionsXMLSerializer<T> : IOptionsSerializer<T> where T: IComparable, new()
    {
        private string defaultOptionsFilePath, userOptionsFilePath;

        /**
         * Konstruktor
         * Neben den default-Optionen kann bei Bedarf eine weitere Userspezifische
         * Einstellung abgespeichert werden.
         * Die Pfade zu den beiden Dateien sind hier zu übergeben.
         */
        public OptionsXMLSerializer(string defaultOptionsFileName, string userOptionsFileName)
        {
            this.defaultOptionsFilePath = OptionsAccess.getFolderPath(ApplicationFolder.Settings) + "\\" + defaultOptionsFileName;
            this.userOptionsFilePath = OptionsAccess.getFolderPath(ApplicationFolder.Settings) + "\\" + userOptionsFileName;
        }

        public void serialize(T options)
        {
            bool persist = true;
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

            //mit den default-Optionen vergleichen
            if (File.Exists(defaultOptionsFilePath))
            {
                FileStream readFileStream = new FileStream(this.defaultOptionsFilePath, FileMode.Open, FileAccess.Read, FileShare.None);
                T defaultOptions = (T)xmlSerializer.Deserialize(readFileStream);
                readFileStream.Close();
                persist = (defaultOptions.CompareTo(options) != 0);
            }

            if (persist)
            {
                TextWriter writeFileStream = new StreamWriter(this.userOptionsFilePath);
                xmlSerializer.Serialize(writeFileStream, options);
                writeFileStream.Close();        
            }
            else
            {
                // Lösche userspezifische Option, falls vorhanden
                if (File.Exists(this.userOptionsFilePath))
                {
                    File.Delete(this.userOptionsFilePath);
                }
            }
        }

        public T deserialize()
        {
            T result;

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

            FileStream readFileStream = null;

            try
            {
                if (File.Exists(userOptionsFilePath))
                {
                    //Abfrage der userspezifischen Optionen
                    readFileStream = new FileStream(this.userOptionsFilePath, FileMode.Open, FileAccess.Read, FileShare.None);
                }
                else
                {
                    //Abfrage der Default-Optionen
                    readFileStream = new FileStream(this.defaultOptionsFilePath, FileMode.Open, FileAccess.Read, FileShare.None);
                }

                result = (T)xmlSerializer.Deserialize(readFileStream);
                readFileStream.Close();

                return result;
            }
            catch (FileNotFoundException)
            {
                //Keine Optionen vorhanden -> Anlegen der Optionen
                T options = new T();
                
                TextWriter writeFileStream = new StreamWriter(this.userOptionsFilePath);
                xmlSerializer.Serialize(writeFileStream, options);
                writeFileStream.Close();

                return options;
            }
        }
    }
}
