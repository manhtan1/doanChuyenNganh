﻿using phim2101.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace phim2101.Areas.Admin.Controllers
{
    public class VeController : Controller
    {
        private DBContext db = new DBContext();
        // GET: Admin/Ve
        public ActionResult Index()
        {
            var phims = db.Phims.Include(p => p.DinhDangPhim).Include(p => p.TheLoaiPhim);
            DateTime currentTime = DateTime.Now;
            DateTime star1 = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 7, 0, 0);
            DateTime end1 = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 14, 0, 0);
            DateTime start2 = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 14, 15, 0);
            DateTime end2 = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 21, 15, 0);
            NhanVien nv = (NhanVien)Session["user"];
            var count = db.PhanQuyens.Count(m => m.MaNV == nv.MaNV && m.IDChucnang == 3);
            if (count == 0)
            {
                return Redirect("/Admin/Baoloi/khongcapquyen");
            }
            else if (nv.Ca == "Ca1")
            {
                if (currentTime < end1 && currentTime > star1)
                {
                    return View(db.ChiTietHDs.Include(n => n.Ve.ChiTietPhong).Include(i => i.Hoadon).ToList());
                }
                else
                {
                    return Redirect("/admin/home/ngoaithoigian");
                }
            }
            else if (nv.Ca == "Ca2")
            {
                if (currentTime < end2 && currentTime > start2)
                {
                    return View(db.ChiTietHDs.Include(n => n.Ve.ChiTietPhong).Include(i => i.Hoadon).ToList());
                }
                else
                {
                    return Redirect("/admin/home/ngoaithoigian");
                }
            }
            return View();
        }
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ve ve = db.Ves.Find(id);
            if (ve == null)
            {
                return HttpNotFound();
            }
            return View(ve);
        }
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Ves/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaVe,MaPhong,MaPhim")] Ve ve)
        {
            if (ModelState.IsValid)
            {
                db.Ves.Add(ve);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ve);
        }

        // GET: Admin/Ves/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ve ve = db.Ves.Find(id);
            if (ve == null)
            {
                return HttpNotFound();
            }
            return View(ve);
        }

        // POST: Admin/Ves/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaVe, MaPhong, MaPhim")] Ve ve)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ve).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ve);
        }

        // GET: Admin/Ves/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ve ve = db.Ves.Find(id);
            if (ve == null)
            {
                return HttpNotFound();
            }
            return View(ve);
        }

        // POST: Admin/Ves/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ve ve = db.Ves.Find(id);
            db.Ves.Remove(ve);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}