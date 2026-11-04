﻿using Newtonsoft.Json.Linq;
using BH_Shortcut.Core.Configurations; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH_Shortcut.Core.Session
{
    public class SessionManager
    {
        public bool IsLive { get; set; } = false;
        public string UUID { get; private set; }
        public ulong UserNo { get; private set; }
        public string ID { get; private set; }
        public string Name { get; private set; }
        public int Timeout { get; private set; }
        public UserType UserType { get; private set; }
        public StatusTypes Status { get; private set; }

        private static SessionManager _instance;
        public static SessionManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    MakeFakeSession();
                }
                return _instance;
            }
        }

        private static void MakeFakeSession()
        {
            _instance = new SessionManager();
            _instance.UUID = "";
            _instance.UserNo = 0;
            _instance.ID = "(KnownID)";
            _instance.Name = "(KnownName)";
            _instance.Status = StatusTypes.사용불가;
            _instance.Timeout = 0;
            _instance.UserType = UserType.사용자;
        }

        public static void MakeSession(UserSessionData userSessionData)
        {
            _instance = new SessionManager();
            _instance.UUID = userSessionData.uuid;
            _instance.UserNo = userSessionData.user_no;
            _instance.ID = userSessionData.user_id;
            _instance.Name = userSessionData.user_name;
            _instance.Status = userSessionData.status;
            _instance.Timeout = userSessionData.timeout;
            _instance.UserType = userSessionData.user_type;
            _instance.IsLive = true;
        }

        public static void Clear()
        { 
            _instance.IsLive = false;
            _instance.UUID = "";
            _instance.UserNo = 0;
            _instance.Status = StatusTypes.사용불가;
        }
    }
}
