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
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using UBT.AI4.Bio.DivMobi.DataLayer.DataItems;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Restrictions;
using System.ComponentModel;

namespace Diversity_Synchronization
{
    public class ProjectSelector : INotifyPropertyChanged
    {
        private const int NoSelection = -1;
        private ObservableCollection<Project> projects;
        private int selectedProjectIndex = NoSelection;
        
        public ProjectSelector()
        {
            fetchProjects();
            var selectedProjects = from p in projects where p.ID == ConnectionsAccess.Profile.ProjectID select p;
            if (selectedProjects.Count() > 0)
            {
                
                CurrentProjectIndex = projects.IndexOf(selectedProjects.First());
            }
            else
            {
                CurrentProjectIndex = -1;
            }
        }

        #region Project Selection
        public class Project
        {
            public int ID { get; set; }
            public string Title { get; set; }
        }
        public ObservableCollection<Project> Projects
        {
            get
            {
                if (projects == null)
                    projects = new ObservableCollection<Project>();
                return projects;
            }
        }

        public int CurrentProjectIndex
        {
            get
            {
                return selectedProjectIndex;
            }
            set
            {
                if (value < projects.Count)
                    selectedProjectIndex = value;
                else
                    selectedProjectIndex = NoSelection;              

                this.RaisePropertyChanged(() => CurrentProjectIndex, PropertyChanged);
            }
        }

        public Project SelectedProject
        {
            get
            {
                if (CurrentProjectIndex != NoSelection)
                    return Projects[CurrentProjectIndex];
                else
                    return null;
            }
        }
        private bool ProjectSelected
        {
            get
            {
                return CurrentProjectIndex > -1;
            }
        }
        #endregion


        private void fetchProjects()
        {
            using (var conn = ConnectionsAccess.RepositoryDB.CreateConnection())
            {
                
                var projectInfoCmd = conn.CreateCommand();
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT * FROM ProjectProxy");
                sb.Append(" WHERE ProjectID IN");
                sb.Append(" (Select ProjectID From ProjectUser Where LoginName='" + ConnectionsAccess.Profile.LoginName + "')");
                projectInfoCmd.CommandText = sb.ToString();
                //projectInfoCmd.CommandText = "SELECT ProjectID,Project,ProjectTitle FROM ProjectList";
                //projectInfoCmd.CommandText = "SELECT ProjectID,Project FROM ProjectList";
                conn.Open();

                using (var projectReader = projectInfoCmd.ExecuteReader())
                {
                    try
                    {
                        
                        int projectIDOrdinal = projectReader.GetOrdinal("ProjectID"),
                            projectOrdinal = projectReader.GetOrdinal("Project");
                            //projectTitleOrdinal = projectReader.GetOrdinal("ProjectTitle");

                        Projects.Clear();
                        while (projectReader.Read())
                        {
                            string projectTitle = /*projectReader.IsDBNull(projectTitleOrdinal) ?*/ projectReader.GetString(projectOrdinal);// : projectReader.GetString(projectTitleOrdinal);
                            int projectID = projectReader.GetInt32(projectIDOrdinal);
                            Projects.Add(new Project() { ID = projectID, Title = projectTitle });
                        }
                    }
                    finally
                    {
                        projectReader.Close();
                    }
                }
                conn.Close();
            }
        }

        

        

        #region INotifyPropertyChanged Member

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
