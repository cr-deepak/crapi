﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataserviceInterface;
using System.Data;
using DataModels;


namespace ApplicationService
{
    public class CommonApplicationService
    {
        Icommonservice _commondataservice;
        private Icommonservice commondataservice
        {
            get { return _commondataservice; }
        }

        public CommonApplicationService(Icommonservice commonservice)
        {
            _commondataservice = commonservice;
        }

        public DataTable getMenu(int profileid)
        {
            var menulist = commondataservice.GetMenu(profileid);
            return menulist;
        }


        public object getProfileList(int userid, int profileid)
        {
            var friendsList = commondataservice.GetProfileList(userid, profileid);
            return friendsList;
        }


        public List<State> GetState(out TransactionalInformation transaction)
        {
            var states = new List<State>();
            transaction = new TransactionalInformation();
            try
            {
                states = commondataservice.GetState();
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;
                transaction.ReturnMessage.Add(errorMessage);
            }

            return states;
        }

        public List<searchuser> GetuserByLocations(string location, int city, int state, out TransactionalInformation transaction)
        {
            var states = new List<searchuser>();
            transaction = new TransactionalInformation();
            try
            {
                states = commondataservice.GetProfileByLocation(location, city, state);
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;
                transaction.ReturnMessage.Add(errorMessage);
            }

            return states;
        }


        public List<City> GetCity(int stateid, out TransactionalInformation transaction)
        {
            var cities = new List<City>();
            transaction = new TransactionalInformation();
            try
            {
                transaction.ReturnStatus = true;
                cities = commondataservice.GetCity(stateid);
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;
                transaction.ReturnStatus = false;
                transaction.ReturnMessage.Add(errorMessage);
            }

            return cities;
        }


        public List<pincode> GetPinCode(string cityid, out TransactionalInformation transaction)
        {
            var pincodes = new List<pincode>();
            transaction = new TransactionalInformation();
            try
            {
                transaction.ReturnStatus = true;
                pincodes = commondataservice.GetPinCode(cityid);
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;
                transaction.ReturnStatus = false;
                transaction.ReturnMessage.Add(errorMessage);
            }

            return pincodes;
        }

    }
}
