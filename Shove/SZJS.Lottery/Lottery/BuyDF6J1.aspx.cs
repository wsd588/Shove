﻿using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;

public partial class Lottery_BuyDF6J1 : SitePageBase
{
    public int LotteryID;
    public string LotteryName;
    public string RepWinNumber;
    public string DZ = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        AjaxPro.Utility.RegisterTypeForAjax(typeof(Lottery_BuyDF6J1), this.Page);
        LotteryID = 58;
        if (!IsPostBack)
        {
            RepNewt.DataSource = GetNews("东方6+1资讯");
            RepNewt.DataBind();
            RepWinNumber = GetWinNumber(LotteryID);

            RepWin.DataSource = DataCache.GetWinInfo(LotteryID);
            RepWin.DataBind();
            hlAgreement.NavigateUrl = "../Home/Room/BuyProtocol.aspx?LotteryID=" + LotteryID;
            DZ = Shove._Security.Encrypt.UnEncryptString(PF.GetCallCert(), Shove._Web.Utility.GetRequest("DZ"));
        }
    }
    /// <summary>
    /// 获取服务器时间
    /// </summary>
    /// <returns></returns>
    [AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.None)]
    public string GetSysTime()
    {
        DateTime now = Shove._Convert.StrToDateTime(new DAL.Views.V_GetDate().Open("", "", "").Rows[0]["Now"].ToString(), DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

        return now.ToString("yyyy/MM/dd HH:mm:ss");
    }

    /// <summary>
    /// 获取期信息
    /// </summary>
    /// <param name="LotteryID"></param>
    /// <returns></returns>
    [AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.None)]
    public string GetIsuseInfo(int LotteryID)
    {
        try
        {
            DataTable dt = DataCache.GetIsusesInfo(LotteryID);
            string strNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            //本期期信息
            DataRow[] drCurrIsuse = dt.Select("'" + strNow + "' > StartTime and '" + strNow + "' < EndTime", "EndTime desc");
            //上期期信息（已开奖）
            DataRow[] drPreIsuse = dt.Select("EndTime < '" + strNow + "' and WinLotteryNumber<>''", "EndTime desc");

            if (drCurrIsuse.Length == 0 && drPreIsuse.Length == 0)
            {
                return "";
            }

            if (drCurrIsuse.Length == 0)
            {
                drCurrIsuse = dt.Select("EndTime < '" + strNow + "'", "EndTime desc");
            }

            if (drPreIsuse.Length == 0)
            {
                drPreIsuse = drCurrIsuse;
            }

            //本期
            int IsuseID = Shove._Convert.StrToInt(drCurrIsuse[0]["ID"].ToString(), -1);
            string IsuseName = drCurrIsuse[0]["Name"].ToString();

            int SystemEndAheadMinute = DataCache.LotteryEndAheadMinute[LotteryID];
            DateTime EndTime = Convert.ToDateTime(drCurrIsuse[0]["EndTime"]);
            DateTime SystemEndTime = EndTime.AddMinutes(SystemEndAheadMinute * -1);

            string IsuseEndTime = SystemEndTime.ToString("yyyy/MM/dd HH:mm:ss");
            //END

            //上期
            string LastIsuseName = drPreIsuse[0]["Name"].ToString();
            string LastWinNumber = drPreIsuse[0]["WinLotteryNumber"].ToString().Trim();

            LastWinNumber = FormatWinNumber(LastWinNumber);

            StringBuilder sb = new StringBuilder();

            sb.Append(IsuseID.ToString())
                .Append(",")
                .Append(IsuseName)
                .Append(",")
                .Append(IsuseEndTime)
                .Append("|")
                .Append(LastIsuseName)
              .Append("|").Append(GetIsuseChase(LotteryID))
              .Append("|")
             .Append(LastWinNumber);

            return sb.ToString();
        }
        catch (Exception e)
        {
            new Log("TWZT").Write(this.GetType() + e.Message);
            return "";
        }
    }
    /// <summary>
    /// 追号信息
    /// </summary>
    /// <param name="LotteryID"></param>
    /// <returns></returns>
    private string GetIsuseChase(int LotteryID)
    {
        try
        {
            DataTable dt = DataCache.GetIsusesInfo(LotteryID);

            int SystemEndAheadMinute = DataCache.LotteryEndAheadMinute[LotteryID];

            DataRow[] drCanChase = dt.Select("('" + DateTime.Now.ToString() + "' < StartTime or ('" + DateTime.Now.ToString() + "'>StartTime and '" + DateTime.Now.AddMinutes(SystemEndAheadMinute).ToString() + "'<EndTime))", "EndTime");

            if (drCanChase.Length == 0)
            {
                return "";
            }

            StringBuilder sb = new StringBuilder();
            int row = 0;
            sb.Append("<table cellpadding='0' cellspacing='1' style='width: 100%; text-align: center; background-color: #E2EAED;'><tbody style='background-color: White;'>");
            foreach (DataRow dr in drCanChase)
            {
                row++;
                sb.Append("<tr>")
                .Append("<td style='width: 10%;'>")
                .Append("<input id='check").Append(dr["ID"].ToString()).Append("' type='checkbox' name='check").Append(dr["ID"].ToString()).Append("' onclick='check(this);' value='").Append(dr["ID"].ToString()).Append("'/>")
                .Append("</td>")
                .Append("<td style='width: 40%;'>")
                .Append("<span>").Append(dr["Name"].ToString()).Append("</span>")
                .Append("<span>").Append(row == 1 ? "<font color='red'>[本期]</font>" : row == 2 ? "<font color='red'>[下期]</font>" : "").Append("</span>")
                .Append("</td>")
                .Append("<td style='width: 20%;'>")
                .Append("<input name='times").Append(dr["ID"].ToString()).Append("' type='text' value='1' id='times").Append(dr["ID"].ToString()).Append("' class='TextBox' onchange='onTextChange(this);' onkeypress='return InputMask_Number();' disabled='disabled' onblur='CheckMultiple2(this);' style='width: 45px;' />倍")
                .Append("</td>")
                .Append("<td style='width: 30%;'>")
                .Append("<input name='money").Append(dr["ID"].ToString()).Append("' type='text' value='0' id='money").Append(dr["ID"].ToString()).Append("' class='TextBox' disabled='disabled' style='width: 45px;' />元")
                .Append("</td>")
                .Append("</tr>");
            }
            sb.Append("</tbody></table>");

            return sb.ToString();
        }
        catch (Exception e)
        {
            new Log("TWZT").Write("AlipayTask Running Error: " + e.Message);
            return "";
        }
    }

    /// <summary>
    /// 获得佣金比例
    /// </summary>
    /// <returns>返回佣金比例</returns>
    [AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.ReadWrite)]
    public string GetSchemeBonusScalec(int lotteryId)
    {
        string bScalec;
        //获得站点选项中的佣金比率
        DataTable dt = new DAL.Tables.T_Sites().Open("Opt_InitiateSchemeBonusScale,Opt_InitiateSchemeLimitLowerScaleMoney,Opt_InitiateSchemeLimitLowerScale,Opt_InitiateSchemeLimitUpperScaleMoney,Opt_InitiateSchemeLimitUpperScale", "", "");
        //把佣金比率换成整数
        bScalec = (Shove._Convert.StrToDouble(dt.Rows[0]["Opt_InitiateSchemeBonusScale"].ToString(), 0) * 100).ToString();

        //发起方案条件
        string Opt_InitiateSchemeLimitLowerScaleMoney = (Shove._Convert.StrToDouble(dt.Rows[0]["Opt_InitiateSchemeLimitLowerScaleMoney"].ToString(), 100)).ToString();
        string Opt_InitiateSchemeLimitLowerScale = (Shove._Convert.StrToDouble(dt.Rows[0]["Opt_InitiateSchemeLimitLowerScale"].ToString(), 0.2)).ToString();
        string Opt_InitiateSchemeLimitUpperScaleMoney = (Shove._Convert.StrToDouble(dt.Rows[0]["Opt_InitiateSchemeLimitUpperScaleMoney"].ToString(), 10000)).ToString();
        string Opt_InitiateSchemeLimitUpperScale = (Shove._Convert.StrToDouble(dt.Rows[0]["Opt_InitiateSchemeLimitUpperScale"].ToString(), 0.05)).ToString();

        string lotteryName = DataCache.Lotteries[lotteryId];

        return bScalec + "|" + Opt_InitiateSchemeLimitLowerScaleMoney + "|" + Opt_InitiateSchemeLimitLowerScale + "|" + Opt_InitiateSchemeLimitUpperScaleMoney + "|" + Opt_InitiateSchemeLimitUpperScale + "|" + lotteryId.ToString() + "|" + lotteryName;
    }

    /// <summary>
    /// 购买
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btn_OK_Click(object sender, EventArgs e)
    {
        if (!PF.IsSystemRegister())
        {
            PF.GoError(ErrorNumber.DataReadWrite, "请联系网站管理员输入软件序列号", this.GetType().BaseType.FullName);
            return;
        }
        Buy(_User);
    }
    /// <summary>
    /// 购买彩票
    /// </summary>
    /// <param name="_User"></param>
    private void Buy(Users _User)
    {
        string HidIsuseID = Shove._Web.Utility.GetRequest("HidIsuseID");
        string HidIsuseEndTime = Shove._Web.Utility.GetRequest("HidIsuseEndTime");
        string playType = Shove._Web.Utility.GetRequest("tbPlayTypeID");
        string Chase = Shove._Web.Utility.GetRequest("Chase");
        string CoBuy = Shove._Web.Utility.GetRequest("CoBuy");
        string tb_Share = Shove._Web.Utility.GetRequest("tb_Share");
        string tb_BuyShare = Shove._Web.Utility.GetRequest("tb_BuyShare");
        string tb_AssureShare = Shove._Web.Utility.GetRequest("tb_AssureShare");
        string tb_OpenUserList = Shove._Web.Utility.GetRequest("tb_OpenUserList");
        string tb_Title = Shove._Web.Utility.GetRequest("tb_Title");
        string tb_Description = Shove._Web.Utility.GetRequest("tb_Description");
        string tbAutoStopAtWinMoney = Shove._Web.Utility.GetRequest("tbAutoStopAtWinMoney");
        string tbSecrecyLevel = Shove._Web.Utility.GetRequest("SecrecyLevel");
        string tb_LotteryNumber = Shove._Web.Utility.FilteSqlInfusion(Request["tb_LotteryNumber"]);
        string tb_hide_SumMoney = Shove._Web.Utility.GetRequest("tb_hide_SumMoney");
        string tb_hide_AssureMoney = Shove._Web.Utility.GetRequest("tb_hide_AssureMoney");
        string tb_hide_SumNum = Shove._Web.Utility.GetRequest("tb_hide_SumNum");
        string HidIsuseCount = Shove._Web.Utility.GetRequest("HidIsuseCount");
        string HidLotteryID = Shove._Web.Utility.GetRequest("HidLotteryID");
        string HidIsAlipay = Shove._Web.Utility.GetRequest("HidIsAlipay");
        string tb_Multiple = Shove._Web.Utility.GetRequest("tb_Multiple");
        string HidIsuseName = Shove._Web.Utility.GetRequest("HidIsuseName");
        string tbPlayTypeName = Shove._Web.Utility.GetRequest("tbPlayTypeName");
        string ChaseBuyedMoney = Shove._Web.Utility.GetRequest("tb_hide_ChaseBuyedMoney");
        string tb_SchemeBonusScale = Shove._Web.Utility.GetRequest("tb_SchemeBonusScale");
        string tb_SchemeBonusScalec = Shove._Web.Utility.GetRequest("tb_SchemeBonusScalec");

        int Price = 2;

        if (tb_Multiple == "")
        {
            tb_Multiple = "1";
        }

        double SumMoney = 0;
        int Share = 0;
        int BuyShare = 0;
        double AssureMoney = 0;
        int Multiple = 0;
        int SumNum = 0;
        short SecrecyLevel = 0;
        int PlayTypeID = 0;
        int LotteryID = 0;
        long IsuseID = 0;
        double AutoStopAtWinMoney = 0;
        double SchemeBonusScale = 0;
        double SchemeBonusScalec = 0;

        try
        {
            SumMoney = double.Parse(tb_hide_SumMoney);
            Share = int.Parse(tb_Share);
            BuyShare = int.Parse(tb_BuyShare);
            AssureMoney = double.Parse(tb_hide_AssureMoney);
            Multiple = int.Parse(tb_Multiple);
            SumNum = int.Parse(tb_hide_SumNum);
            SecrecyLevel = short.Parse(tbSecrecyLevel);
            PlayTypeID = int.Parse(playType);
            LotteryID = int.Parse(HidLotteryID);
            IsuseID = long.Parse(HidIsuseID);
            AutoStopAtWinMoney = double.Parse(tbAutoStopAtWinMoney);
            SchemeBonusScale = double.Parse(tb_SchemeBonusScale);
            SchemeBonusScalec = double.Parse(tb_SchemeBonusScalec);
        }
        catch
        {
            Shove._Web.JavaScript.Alert(this.Page, "输入有错误，请仔细检查。");

            return;
        }

        if ((SumMoney <= 0) || (SumNum < 1))
        {
            Shove._Web.JavaScript.Alert(this.Page, "输入有错误，请仔细检查。");

            return;
        }

        if (AssureMoney < 0)
        {
            Shove._Web.JavaScript.Alert(this.Page, "输入有错误，请仔细检查。");

            return;
        }

        if (Share < 1)
        {
            Shove._Web.JavaScript.Alert(this.Page, "输入有错误，请仔细检查。");

            return;
        }

        if ((BuyShare == Share) && (AssureMoney == 0))
        {
            Share = 1;
            BuyShare = 1;
        }

        if ((SumMoney / Share) < 1)
        {
            Shove._Web.JavaScript.Alert(this.Page, "每份金额最低不能少于 1 元。");

            return;
        }

        double BuyMoney = BuyShare * (SumMoney / Share) + AssureMoney;

        if (Chase != "")
        {
            BuyMoney = double.Parse(ChaseBuyedMoney);
        }

        if (BuyMoney > _User.Balance)
        {
            SaveDataForAliBuy();

            return;
        }

        if (BuyMoney > PF.SchemeMaxBettingMoney)
        {
            Shove._Web.JavaScript.Alert(this.Page, "投注金额不能大于" + PF.SchemeMaxBettingMoney.ToString() + "，谢谢。");

            return;
        }

        if (Multiple > 999)
        {
            Shove._Web.JavaScript.Alert(this.Page, "投注倍数不能大于 999 倍，谢谢。");

            return;
        }
        //佣金比例的计算

        if (!(SchemeBonusScale >= 0 || SchemeBonusScale <= 10))
        {
            Shove._Web.JavaScript.Alert(this.Page, "佣金比例只能在0~10之间");

            return;
        }

        if (SchemeBonusScale.ToString().IndexOf("-") > -1 || SchemeBonusScale.ToString().IndexOf(".") > -1)
        {
            Shove._Web.JavaScript.Alert(this.Page, "佣金比例输入有误");

            return;
        }
        if (!(SchemeBonusScalec >= 0 || SchemeBonusScalec <= 10))
        {
            Shove._Web.JavaScript.Alert(this.Page, "佣金比例只能在0~10之间");

            return;
        }

        if (SchemeBonusScalec.ToString().IndexOf("-") > -1 || SchemeBonusScalec.ToString().IndexOf(".") > -1)
        {
            Shove._Web.JavaScript.Alert(this.Page, "佣金比例输入有误");

            return;
        }

        SchemeBonusScale = SchemeBonusScale / 100;
        SchemeBonusScalec = SchemeBonusScalec / 100;

        string LotteryNumber = tb_LotteryNumber;

        if (LotteryNumber[LotteryNumber.Length - 1] == '\n')
        {
            LotteryNumber = LotteryNumber.Substring(0, LotteryNumber.Length - 1);
        }

        #region 对彩票号码进行分析，判断注数

        SZJS.Lottery slsLottery = new SZJS.Lottery();
        string[] t_lotterys = SplitLotteryNumber(LotteryNumber);

        if ((t_lotterys == null) || (t_lotterys.Length < 1))
        {
            Shove._Web.JavaScript.Alert(this.Page, "选号发生异常，请重新选择号码投注，谢谢。(-694)");

            return;
        }

        int ValidNum = 0;

        foreach (string str in t_lotterys)
        {
            string Number = slsLottery[LotteryID].AnalyseScheme(str, PlayTypeID);

            if (string.IsNullOrEmpty(Number))
            {
                continue;
            }

            string[] str_s = Number.Split('|');

            if (str_s == null || str_s.Length < 1)
            {
                continue;
            }

            ValidNum += Shove._Convert.StrToInt(str_s[str_s.Length - 1], 0);
        }

        if (ValidNum != SumNum)
        {
            Shove._Web.JavaScript.Alert(this.Page, "选号发生异常，请重新选择号码投注，谢谢。");

            return;
        }

        #endregion

        StringBuilder ChaseXML = new StringBuilder();
        int RpTodayDataCount = 0;
        string AdditionasXml = "";
        string ReturnDescription = "";

        //追号
        if (Chase == "1")
        {
            foreach (string key in Request.Form.AllKeys)
            {
                if (key.IndexOf("check") > -1)
                {
                    int row = Shove._Convert.StrToInt(key.Replace("check", ""), -1);
                    if (row > 0)
                    {
                        RpTodayDataCount++;
                        int money = Shove._Convert.StrToInt(Request.Form["tb_hide_SumNum"], -1) * Price * Shove._Convert.StrToInt(Request.Form["times" + row.ToString()], -1);
                        ChaseXML.Append(Request.Form[key]).Append(",")
                            .Append(Request.Form["times" + row.ToString()]).Append(",")
                            .Append(money.ToString()).Append(";");
                    }
                }
            }

            if (ChaseXML.Length > 0)
            {
                ChaseXML.Remove(ChaseXML.Length - 1, 1);
            }

            if (LotteryNumber[LotteryNumber.Length - 1] == '\n')
            {
                LotteryNumber = LotteryNumber.Substring(0, LotteryNumber.Length - 1);
            }

            try
            {
                SumMoney = double.Parse(tb_hide_SumMoney);
            }
            catch
            {
                Shove._Web.JavaScript.Alert(this.Page, "输入有错误，请仔细检查。(-1325)");

                return;
            }

            if (SumMoney < 2)
            {
                Shove._Web.JavaScript.Alert(this.Page, "输入有错误，请仔细检查。(-1332)");

                return;
            }

            string[] XML = ChaseXML.ToString().Split(';');
            int CompetitionCount = XML.Length;

            string[] Xmlparams = new string[CompetitionCount * 6];

            string str_EndTime = DAL.Functions.F_GetIsuseSystemEndTime(long.Parse(XML[0].Split(',')[0]), PlayTypeID).ToString();
            DateTime EndTime = DateTime.Parse(str_EndTime);

            if (DateTime.Now >= EndTime)
            {
                Shove._Web.JavaScript.Alert(this.Page, "您选择的追号期号中包含已截止的期，请重新选择。");

                return;
            }

            //构建格式：期号,玩法类别,方案,倍数,金额,方案保密级别
            for (int i = 0; i < CompetitionCount; i++)
            {
                Xmlparams[i * 6] = XML[i].Split(',')[0];        //期号
                Xmlparams[i * 6 + 1] = PlayTypeID.ToString();   //玩法类别
                Xmlparams[i * 6 + 2] = LotteryNumber;           //方案
                Xmlparams[i * 6 + 3] = XML[i].Split(',')[1];    //倍数
                Xmlparams[i * 6 + 4] = XML[i].Split(',')[2];    //金额
                Xmlparams[i * 6 + 5] = SecrecyLevel.ToString();

                if (Shove._Convert.StrToDouble(Xmlparams[i * 6 + 3], 0) * SumMoney != Shove._Convert.StrToDouble(Xmlparams[i * 6 + 4], 1))
                {
                    Shove._Web.JavaScript.Alert(this.Page, "输入有错误，请仔细检查。");

                    return;
                }

                if (Shove._Convert.StrToDouble(Xmlparams[i * 6 + 3], 0) < Multiple)
                {
                    Shove._Web.JavaScript.Alert(this.Page, "追号倍数有错误，请仔细检查！");

                    return;
                }

                if (double.Parse(Xmlparams[i * 6 + 3]) * SumNum * Price != double.Parse(Xmlparams[i * 6 + 4]))
                {
                    Shove._Web.JavaScript.Alert(this.Page, "追号金额有错误，请仔细检查！可能原因：浏览器不兼容，建议使用IE 7.0");

                    return;
                }
            }

            AdditionasXml = PF.BuildIsuseAdditionasXmlForChase(Xmlparams);

            if (AdditionasXml == "")
            {
                Shove._Web.JavaScript.Alert(this.Page, "追号发生错误。");

                return;
            }

            if (_User.InitiateChaseTask(tb_Title.Trim(), tb_Description.Trim(), LotteryID, AutoStopAtWinMoney, AdditionasXml, LotteryNumber, SchemeBonusScalec, ref ReturnDescription) < 0)
            {
                PF.GoError(ErrorNumber.Unknow, ReturnDescription, this.GetType().FullName + "(-754)");

                return;
            }

            Shove._Web.Cache.ClearCache("Home_Room_CoBuy_BindDataForType" + IsuseID.ToString());
            Shove._Web.Cache.ClearCache("Home_Room_SchemeAll_BindData" + IsuseID.ToString());
            Shove._Web.Cache.ClearCache(_Site.ID.ToString() + "AccountFreezeDetail_" + _User.ID.ToString());

            Response.Redirect("../Home/Room/UserBuySuccess.aspx?LotteryID=" + LotteryID.ToString() + "&Type=2&Money=" + BuyMoney.ToString() + "");

            return;
        }
        else
        {
            if (DateTime.Now >= Shove._Convert.StrToDateTime(HidIsuseEndTime.Replace("/", "-"), DateTime.Now.AddDays(-1).ToString()))
            {
                Shove._Web.JavaScript.Alert(this.Page, "本期投注已截止，谢谢。");

                return;
            }

            if (Price * SumNum * Multiple != SumMoney)
            {
                Shove._Web.JavaScript.Alert(this.Page, "输入有错误，请仔细检查。");

                return;
            }

            long SchemeID = _User.InitiateScheme(IsuseID, PlayTypeID, tb_Title.Trim() == "" ? "(无标题)" : tb_Title.Trim(), tb_Description.Trim(), LotteryNumber, "", Multiple, SumMoney, AssureMoney, Share, BuyShare, tb_OpenUserList.Trim(), short.Parse(SecrecyLevel.ToString()), SchemeBonusScale, ref ReturnDescription);
            if (SchemeID < 0)
            {
                PF.GoError(ErrorNumber.Unknow, ReturnDescription, this.GetType().FullName + "(-755)");

                return;
            }

            Shove._Web.Cache.ClearCache("Home_Room_CoBuy_BindDataForType" + IsuseID.ToString());
            Shove._Web.Cache.ClearCache("Home_Room_SchemeAll_BindData" + IsuseID.ToString());

            if (SumMoney > 50 && Share > 1)
            {
                Shove._Web.Cache.ClearCache("Home_Room_JoinAllBuy_BindData");
            }
            Response.Redirect("../Home/Room/UserBuySuccess.aspx?LotteryID=" + LotteryID.ToString() + "&&Money=" + BuyMoney.ToString() + "&SchemeID=" + SchemeID.ToString() + "");

            return;
        }
    }
    /// <summary>
    /// 余额不足时，保存数据
    /// </summary>
    private void SaveDataForAliBuy()
    {
        string HidIsuseID = Shove._Web.Utility.GetRequest("HidIsuseID");
        string HidIsuseEndTime = Shove._Web.Utility.GetRequest("HidIsuseEndTime");
        string playType = Shove._Web.Utility.GetRequest("tbPlayTypeID");
        string Chase = Shove._Web.Utility.GetRequest("Chase");
        string Cobuy = Shove._Web.Utility.GetRequest("Cobuy");
        string tb_Share = Shove._Web.Utility.GetRequest("tb_Share");
        string tb_BuyShare = Shove._Web.Utility.GetRequest("tb_BuyShare");
        string tb_AssureShare = Shove._Web.Utility.GetRequest("tb_AssureShare");
        string tb_OpenUserList = Shove._Web.Utility.GetRequest("tb_OpenUserList");
        string tb_Title = Shove._Web.Utility.GetRequest("tb_Title");
        string tb_Description = Shove._Web.Utility.GetRequest("tb_Description");
        string tbAutoStopAtWinMoney = Shove._Web.Utility.GetRequest("tbAutoStopAtWinMoney");
        string tbSecrecyLevel = Shove._Web.Utility.GetRequest("SecrecyLevel");
        string tbPlayTypeName = Shove._Web.Utility.GetRequest("tbPlayTypeName");
        string tb_LotteryNumber = Shove._Web.Utility.FilteSqlInfusion(Request["tb_LotteryNumber"]);
        string tb_hide_SumMoney = Shove._Web.Utility.GetRequest("tb_hide_SumMoney");
        string tb_hide_AssureMoney = Shove._Web.Utility.GetRequest("tb_hide_AssureMoney");
        string tb_hide_SumNum = Shove._Web.Utility.GetRequest("tb_hide_SumNum");
        string HidIsuseCount = Shove._Web.Utility.GetRequest("HidIsuseCount");
        string HidLotteryID = Shove._Web.Utility.GetRequest("HidLotteryID");
        string HidIsAlipay = Shove._Web.Utility.GetRequest("HidIsAlipay");
        string tb_Multiple = Shove._Web.Utility.GetRequest("tb_Multiple");
        string AdditionasXml = "";
        StringBuilder ChaseXML = new StringBuilder();
        int RpTodayDataCount = 0;

        int Price = 2;

        if (tb_Multiple == "")
        {
            tb_Multiple = "1";
        }

        if (Chase == "1")
        {
            foreach (string key in Request.Form.AllKeys)
            {
                if (key.IndexOf("check") > -1)
                {
                    int row = Shove._Convert.StrToInt(key.Replace("check", ""), -1);
                    if (row > 0)
                    {
                        RpTodayDataCount++;
                        int money = Shove._Convert.StrToInt(tb_hide_SumNum, -1) * Price * Shove._Convert.StrToInt(Request.Form["times" + row.ToString()], -1);
                        ChaseXML.Append(Request.Form[key]).Append(",")
                            .Append(Request.Form["times" + row.ToString()]).Append(",")
                            .Append(money.ToString()).Append(";");
                    }
                }
            }

            string LotteryNumber = tb_LotteryNumber;

            if (ChaseXML.Length > 0)
            {
                ChaseXML.Remove(ChaseXML.Length - 1, 1);
            }

            if (LotteryNumber[LotteryNumber.Length - 1] == '\n')
            {
                LotteryNumber = LotteryNumber.Substring(0, LotteryNumber.Length - 1);
            }

            string[] XML = ChaseXML.ToString().Split(';');
            int CompetitionCount = XML.Length;

            string[] Xmlparams = new string[CompetitionCount * 6];

            string str_EndTime = DAL.Functions.F_GetIsuseSystemEndTime(long.Parse(XML[0].Split(',')[0]), int.Parse(playType)).ToString();
            DateTime EndTime = DateTime.Parse(str_EndTime);

            if (DateTime.Now >= EndTime)
            {
                Shove._Web.JavaScript.Alert(this.Page, "您选择的追号期号中包含已截止的期，请重新选择。");

                return;
            }

            //构建格式：期号,玩法类别,方案,倍数,金额,方案保密级别
            for (int i = 0; i < CompetitionCount; i++)
            {
                Xmlparams[i * 6] = XML[i].Split(',')[0];        //期号
                Xmlparams[i * 6 + 1] = playType;   //玩法类别
                Xmlparams[i * 6 + 2] = LotteryNumber;           //方案
                Xmlparams[i * 6 + 3] = XML[i].Split(',')[1];    //倍数
                Xmlparams[i * 6 + 4] = XML[i].Split(',')[2];    //金额
                Xmlparams[i * 6 + 5] = tbSecrecyLevel;
            }

            AdditionasXml = PF.BuildIsuseAdditionasXmlForChase(Xmlparams);
        }

        DAL.Tables.T_AlipayBuyTemp tbp = new DAL.Tables.T_AlipayBuyTemp();

        tbp.SiteID.Value = 1;
        tbp.UserID.Value = -1;
        tbp.Money.Value = tb_hide_SumMoney;
        tbp.HandleResult.Value = 0;
        tbp.IsChase.Value = Chase == "1";
        tbp.IsCoBuy.Value = Cobuy == "2";
        tbp.LotteryID.Value = HidLotteryID;
        tbp.IsuseID.Value = HidIsuseID;
        tbp.PlayTypeID.Value = playType;
        tbp.StopwhenwinMoney.Value = tbAutoStopAtWinMoney;
        tbp.AdditionasXml.Value = AdditionasXml;
        tbp.Title.Value = tb_Title;
        tbp.Description.Value = tb_Description;
        tbp.LotteryNumber.Value = tb_LotteryNumber;
        tbp.UpdateloadFileContent.Value = "";
        tbp.Multiple.Value = tb_Multiple;
        tbp.BuyMoney.Value = tb_BuyShare;
        tbp.SumMoney.Value = tb_hide_SumMoney;
        tbp.AssureMoney.Value = tb_hide_AssureMoney;
        tbp.Share.Value = tb_Share;
        tbp.BuyShare.Value = tb_BuyShare;
        tbp.AssureShare.Value = tb_AssureShare;
        tbp.OpenUsers.Value = tb_OpenUserList;
        tbp.SecrecyLevel.Value = tbSecrecyLevel;

        long Result = tbp.Insert();

        if (Result < 0)
        {
            new Log("System").Write("T_AlipayBuyTemp 数据库读写错误。");
        }

        //Shove._Web.JavaScript.Alert(this.Page, "您的账户余额不足，请先充值，谢谢。", "../Home/Room/OnlinePay/Default.aspx?BuyID=" + Result.ToString());
    }
    private string[] SplitLotteryNumber(string Number)
    {
        string[] s = Number.Split('\n');
        if (s.Length == 0)
            return null;
        for (int i = 0; i < s.Length; i++)
            s[i] = s[i].Trim();
        return s;
    }
    /// <summary>
    /// 获取彩票新闻
    /// </summary>
    /// <returns></returns>
    private DataTable GetNews(string TypeName)
    {
        string sql = @"select top 10 ID,Title,Content,TypeName from V_News where isShow = 1  and [TypeName] = '" + TypeName + "'order by isCommend,ID desc";
        DataTable dt = Shove.Database.MSSQL.Select(sql);
        return dt;
    }
    /// <summary>
    /// 每个彩种的开奖号码
    /// </summary>
    /// <param name="lotteryID"></param>
    /// <returns></returns>
    public string GetWinNumber(int lotteryID)
    {
        DataTable dtIsuses = DataCache.GetWinNumber(lotteryID);
        StringBuilder sb = new StringBuilder();
        if (dtIsuses != null && dtIsuses.Rows.Count > 0)
        {
            foreach (DataRow dr in dtIsuses.Rows)
            {
                sb.AppendLine("<tr>")
               .AppendLine("<td>")
               .AppendLine(dr["Name"].ToString())
               .AppendLine("</td>");

                sb.AppendLine("<td class=\"red\">")
                .AppendLine(dr["WinLotteryNumber"].ToString())
                .AppendLine("</td>")
                .AppendLine("</tr>");
            }
        }
        else
        {
            sb.AppendLine("<tr>")
                 .AppendLine("<td colspan=\"2\" class=\"red\">没有数据显示！")
                 .AppendLine("</td>")
            .AppendLine("</tr>");
        }
        return sb.ToString();
    }
    /// <summary>
    /// 格式化开奖号码
    /// </summary>
    /// <param name="LotteryID"></param>
    /// <param name="winNumber"></param>
    /// <returns></returns>
    /// 
    private string FormatWinNumber(string winNumber)
    {
        StringBuilder sb = new StringBuilder();

        if (winNumber.IndexOf(" + ") > 0)
        {
            winNumber = winNumber.Replace(" + ", " ");

            string[] number = winNumber.Split(' ');

            for (int i = 0; i < number.Length; i++)
            {
                if (i < number.Length - 1)
                {
                    sb.Append(" <span>" + number[i] + "</span>");
                }
                else
                {
                    sb.Append("  <span style=\"background-image: url('../Home/Room/Images/zfb_blueball.gif'); margin-left: 5px;\">" + number[i] + "</span>");
                }
            }
        }

        return sb.ToString();
    }
}
