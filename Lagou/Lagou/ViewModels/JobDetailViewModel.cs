﻿using Lagou.API.Entities;
using Lagou.API.Methods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lagou.ViewModels {
    public class JobDetailViewModel : BaseVM {
        public override string Title {
            get {
                return "职位详情";
            }
        }

        public Position Data { get; set; }

        public int ID { get; set; }

        protected async override void OnActivate() {
            base.OnActivate();

            var mth = new PositionDetail() {
                PositionID = this.ID
            };
            this.Data = await API.ApiClient.Execute(mth);
            this.NotifyOfPropertyChange(() => this.Data);
        }
    }
}
