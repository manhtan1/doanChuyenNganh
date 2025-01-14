﻿using phim2101.Common;
using phim2101.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;

using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace phim2101.Controllers
{
    public class DatveController : Controller
    {
        private DBContext db = new DBContext();
        public ActionResult TrangDatVe(int id, int phong)
        {
            Phim phim1 = db.Phims.Find(id);

            var p1 = db.ChiTietPhongs.Include(n=>n.Phim).SingleOrDefault(n=>n.MaPhim==id && n.MaPhong==phong );

            if (p1 == null)
            {
                return HttpNotFound();
            }
            if (Session["TaiKhoan"] == null || Session["TaiKhoan"].ToString() == "")
            {
                return RedirectToAction("DangNhap", "Home");
            }

            /*foreach (var i in p1)
            {
                return View(i);
            }*/
            return View(p1);
        }
        public List<GioHangItem> Laygiohang()
        {
            List<GioHangItem> lstgiohang = Session["GioHang"] as List<GioHangItem>;
            if (lstgiohang == null)
            {
                lstgiohang = new List<GioHangItem>();
                Session["GioHang"] = lstgiohang;
            }
            return lstgiohang;
        }
        public ActionResult Themgiohang(int MaPhim, int idphong, string ghe, string strURL)
        {
            List<GioHangItem> lstgiohang = Laygiohang();
            GioHangItem product = lstgiohang.Find(n => n.MaPhim == MaPhim);
            var site = ghe;
            if (product == null)
            {
                product = new GioHangItem(MaPhim, idphong);
                product.Ghe = site;
                lstgiohang.Add(product);
                return Redirect(strURL);
            }
            else
            {
                product.Ghe = product.Ghe + ", " + site;
                product.SoLuong += 1;
                lstgiohang.Add(product);
                return Redirect(strURL);
            }
        }
        public ActionResult Datve(int? id)
        {
            List<GioHangItem> lstgiohang = Session["GioHang"] as List<GioHangItem>;
            GioHangItem gh = lstgiohang.Find(n => n.MaPhim == id);
            return View(gh);
        }

        [HttpPost]
        public ActionResult Datve(int id)
        {
            //db.SaveChanges();
            return RedirectToAction("index", "home");
        }
        public ActionResult datvee(int? id)
        {
            List<GioHangItem> lstgiohang = Session["GioHang"] as List<GioHangItem>;
            GioHangItem gh = lstgiohang.Find(n => n.MaPhim == id);
            return View(gh);
        }
        [HttpPost]
        public ActionResult datvee(int id)
        {
            Random rnd = new Random();
            int mave = rnd.Next(1, 1000);
            int mahd = rnd.Next(1, 1000);
            List<GioHangItem> lstgiohang = Session["GioHang"] as List<GioHangItem>;
            var session = (KhachHang)Session["TaiKhoan"];
            GioHangItem gh = lstgiohang.Find(n => n.MaPhim == id);
            Phim phim = db.Phims.Find(id);
            Ve ve = new Ve();
            ve.MaPhim = gh.MaPhim;
            ve.MaPhong = gh.MaPhong;
            db.Ves.Add(ve);
            Hoadon hd = new Hoadon();
            hd.MaKH = session.MaKH;
            hd.TongTien = gh.dthanhtien;
            hd.MaNV = 1;
            db.Hoadons.Add(hd);
            ChiTietHD cthd = new ChiTietHD();
            cthd.MaHD = hd.MaHD;
            cthd.MaVe = ve.MaVe;
            cthd.MaUD = 1;
            cthd.MaDV = 1;
            cthd.Ghe = gh.Ghe;
            cthd.SoLuong = gh.SoLuong;
            cthd.NgayBanVe = DateTime.Now;
            db.ChiTietHDs.Add(cthd);
            Session["GioHang"] = null;
            db.SaveChanges();
            string content = System.IO.File.ReadAllText(Server.MapPath("~/Assets/Customer/template/dathang.html"));
            var toEmail = ConfigurationManager.AppSettings["ToEmailAddress"].ToString();
            foreach (var i in db.KhachHangs.ToList())
            {
                string suatchieu = " Thời gian phát :" + gh.SuatChieu;
                content = content.Replace("{{notice}}", "Bạn đã đặt vé thành công!!!!");
                content = content.Replace("{{tenphim}}", phim.TenPhim);
                content = content.Replace("{{suatchieu}}", suatchieu);
                content = content.Replace("{{ghe}}", gh.Ghe);
                content = content.Replace("{{ghe}}", gh.Ghe);
                new MailHelper().SendMail(i.Email, "Đặt vé thành công!!", content);
                new MailHelper().SendMail(toEmail, "Đặt vé thành công!!", content);
            }

            return RedirectToAction("index", "home");
        }

    }
}