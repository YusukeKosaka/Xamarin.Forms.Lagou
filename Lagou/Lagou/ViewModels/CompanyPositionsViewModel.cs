﻿using Caliburn.Micro;
using Caliburn.Micro.Xamarin.Forms;
using Lagou.API.Entities;
using Lagou.API.Methods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Lagou.ViewModels {
    /// <summary>
    /// 公司职位列表
    /// </summary>
    [Regist(InstanceMode.Singleton)]
    public class CompanyPositionsViewModel : BaseVM {
        public override string Title {
            get {
                return "职位列表";
            }
        }

        /// <summary>
        /// 职位类型列表
        /// </summary>
        public List<string> PositionTypes { get; set; } = Enum.GetNames(typeof(PositionTypes)).ToList();

        public string SelectedPositionType { get; set; }

        private int? OldCompanyID = null;
        public int CompanyID { get; set; }

        public string CompanyName { get; set; }

        public string CompanyLogo { get; set; }


        public BindableCollection<PositionBrief> Datas { get; set; } = new BindableCollection<PositionBrief>();

        public PositionBrief SelectedPosition { get; set; }

        public ICommand PositionTypesChangedCmd { get; set; }

        private int Page = 1;

        private INavigationService NS = null;

        public CompanyPositionsViewModel(INavigationService ns) {
            this.NS = ns;

            this.PositionTypesChangedCmd = new Command(async (o) => {
                await this.SetPosType((string)o);
            });
        }

        protected async override void OnActivate() {
            if (this.OldCompanyID != this.CompanyID || this.Datas.Count == 0) {
                // Becase it's singletone, so need clear datas when it show.
                this.Datas.Clear();

                await Task.Delay(500).ContinueWith(t => this.SetPosType(this.PositionTypes.First()));
                this.OldCompanyID = this.CompanyID;
            }
        }

        private async Task LoadPosByType() {
            this.IsBusy = true;

            var method = new PositionList() {
                CompanyID = this.CompanyID,
                PositionType = (PositionTypes)Enum.Parse(typeof(PositionTypes), this.SelectedPositionType),
                Page = this.Page
            };
            var datas = await API.ApiClient.Execute(method);
            if (!method.HasError && datas.Count() > 0) {
                this.Page++;
                this.Datas.AddRange(datas);
            }

            this.IsBusy = false;
        }

        private async Task SetPosType(string type) {
            this.SelectedPositionType = type;
            this.Page = 1;
            this.Datas.Clear();
            await this.LoadPosByType();
        }

        public void ShowPosition() {
            this.NS
                .For<JobDetailViewModel>()
                .WithParam(p => p.ID, this.SelectedPosition.PositionId)
                .Navigate();

        }
    }
}
