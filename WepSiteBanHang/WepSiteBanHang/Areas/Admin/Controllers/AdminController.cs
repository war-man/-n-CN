﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WepSiteBanHang.Models;
namespace WepSiteBanHang.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
        dbQuanLyEntities db = new dbQuanLyEntities();

        // GET: Admin/Admin

        public ActionResult Index()
        {
            return View(db.SanPhams.OrderByDescending(n => n.MaSP));
         
        }
        [HttpGet]
        public ActionResult TaoMoi()
        {
            //Load dropdownlist nhà cung cấp và dropdownlist loại SP
            ViewBag.MaNCC = new SelectList(db.NhaCungCaps.OrderBy(n => n.TenNCC), "MaNCC", "TenNCC");
            ViewBag.MaLoaiSP = new SelectList(db.LoaiSanPhams.OrderBy(n => n.MaLoaiSP), "MaLoaiSP", "TenLoai");
            ViewBag.MaNSX = new SelectList(db.NhaSanXuats.OrderBy(n => n.MaNSX), "MaNSX", "TenNSX");
            //Kiểm tra hình ảnh tồn tại chưa

            return View();
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult TaoMoi(SanPham sp, HttpPostedFileBase[] HinhAnh)
        {
            ViewBag.MaNCC = new SelectList(db.NhaCungCaps.OrderBy(n => n.TenNCC), "MaNCC", "TenNCC");
            ViewBag.MaLoaiSP = new SelectList(db.LoaiSanPhams.OrderBy(n => n.MaLoaiSP), "MaLoaiSP", "TenLoai");
            ViewBag.MaNSX = new SelectList(db.NhaSanXuats.OrderBy(n => n.MaNSX), "MaNSX", "TenNSX");
            //Kiểm tra hình ảnh tồn tại chưa
            if (HinhAnh[0].ContentLength > 0)
            {
                //Lấy tên hình ảnh
                var fileName = Path.GetFileName(HinhAnh[0].FileName);
                //Lấy hình ảnh chuyển vào thư mục hình ảnh
                var path = Path.Combine(Server.MapPath("~/Content/HinhAnhSP"), fileName);
                //Nếu hình ảnh có rồi thì xuất ra thoonh báo
                if (System.IO.File.Exists(path))
                {
                    ViewBag.upload = "Hình ảnh đã tồn tại";
                    return View();

                }
                else
                {
                    //Lây hình ảnh đưa vào thư mục hình ảnh SP
                    HinhAnh[0].SaveAs(path);

                    sp.HinhAnh = fileName;
                }


            }
            db.SanPhams.Add(sp);
            db.SaveChanges();
            return RedirectToAction("Index");

        }

        [HttpGet]
        public ActionResult ChinhSua(int? id)
        {
            //Lấy sản phẩm cần chỉnh sửa dựa vào id
            if (id == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == id);
            if (sp == null)
            {
                return HttpNotFound();
            }

            //Load dropdownlist nhà cung cấp và dropdownlist loại sp, mã nhà sản xuất
            ViewBag.MaNCC = new SelectList(db.NhaCungCaps.OrderBy(n => n.TenNCC), "MaNCC", "TenNCC", sp.MaNCC);
            ViewBag.MaLoaiSP = new SelectList(db.LoaiSanPhams.OrderBy(n => n.MaLoaiSP), "MaLoaiSP", "TenLoai", sp.MaLoaiSP);
            ViewBag.MaNSX = new SelectList(db.NhaSanXuats.OrderBy(n => n.MaNSX), "MaNSX", "TenNSX", sp.MaNSX);
            return View(sp);
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult ChinhSua(SanPham model, HttpPostedFileBase fileUpload, int id)
        {
            ViewBag.MaNCC = new SelectList(db.NhaCungCaps.OrderBy(n => n.TenNCC), "MaNCC", "TenNCC", model.MaNCC);
            ViewBag.MaLoaiSP = new SelectList(db.LoaiSanPhams.OrderBy(n => n.MaLoaiSP), "MaLoaiSP", "TenLoai", model.MaLoaiSP);
            ViewBag.MaNSX = new SelectList(db.NhaSanXuats.OrderBy(n => n.MaNSX), "MaNSX", "TenNSX", model.MaNSX);
            //Nếu dữ liệu đầu vào chắn chắn ok 
            var sua = db.SanPhams.First(n => n.MaSP == id);


            if (fileUpload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh bìa";
                UpdateModel(sua);
                db.SaveChanges();
                return this.ChinhSua(id);
            }

            else
            {
                if (ModelState.IsValid)
                {

                    var fileName = Path.GetFileName(fileUpload.FileName);

                    var path = Path.Combine(Server.MapPath("~/Content/HinhAnhSP"), fileName);

                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";

                    }
                    else
                    {
                        //Luu hinh anh vao duong dan
                        fileUpload.SaveAs(path);
                    }
                    sua.HinhAnh = fileName;
                    sua.MaSP = id;
                    UpdateModel(sua);
                    db.SaveChanges();
                }

                return this.ChinhSua(id);


            }
        }


        [HttpGet]
        public ActionResult Xoa(int? id)
        {

            //Lấy sản phẩm cần chỉnh sửa dựa vào id
            if (id == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == id);
            if (sp == null)
            {
                return HttpNotFound();
            }

            //Load dropdownlist nhà cung cấp và dropdownlist loại sp, mã nhà sản xuất
            ViewBag.MaNCC = new SelectList(db.NhaCungCaps.OrderBy(n => n.TenNCC), "MaNCC", "TenNCC", sp.MaNCC);
            ViewBag.MaLoaiSP = new SelectList(db.LoaiSanPhams.OrderBy(n => n.MaLoaiSP), "MaLoaiSP", "TenLoai", sp.MaLoaiSP);
            ViewBag.MaNSX = new SelectList(db.NhaSanXuats.OrderBy(n => n.MaNSX), "MaNSX", "TenNSX", sp.MaNSX);
            return View(sp);
        }

        [HttpPost]
        public ActionResult Xoa(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SanPham model = db.SanPhams.SingleOrDefault(n => n.MaSP == id);
            if (model == null)
            {
                return HttpNotFound();
            }
            db.SanPhams.Remove(model);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


    
    [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
       [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            var tk = collection["taikhoan"];
            var mk = collection["password"];
            if (String.IsNullOrEmpty(tk))
            {
                ViewData["Loi1"] = "Chưa nhập tài khoản";
            }
            else
            {
                if (String.IsNullOrEmpty(mk))
                {
                    ViewData["Loi2"] = "Chưa nhập mật khẩu";
                }
                else
                {
                    QuanLy ad = db.QuanLies.SingleOrDefault(m => m.TenTK == tk && m.Pass == mk);
                    if (ad != null)
                    {
                        Session["Admin"] = ad;
                        return RedirectToAction("Index","Admin");
                    }
                    else
                    {
                        ViewData["Loi2"] = "Tài khoản hoặc mật khẩu sai";
                    }
                }
            }

            return RedirectToAction("Index", "Admin");
        }
    }
}