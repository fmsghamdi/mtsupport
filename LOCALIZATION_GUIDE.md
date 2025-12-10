# Complete Localization Guide (English & Arabic)
## Mabat Support System - Globalization & RTL Implementation

---

## ✅ **What's Already Implemented:**

1. ✅ **Program.cs** - Localization configured with Cookie support
2. ✅ **SharedResource.cs** - Centralized resource class
3. ✅ **_Layout.cshtml** - Dynamic RTL/LTR with Bootstrap switching
4. ✅ **_SelectLanguagePartial.cshtml** - Language switcher buttons
5. ✅ **SetLanguage.cshtml/.cs** - Cookie-based language switching

---

## 📝 **Step-by-Step: Creating Resource Files**

### **Why Resource Files?**
Resource files (`.resx`) store translations as key-value pairs. Instead of hardcoding text, you reference keys that map to different languages.

---

## **STEP 1: Create Resource Files in Visual Studio**

### **Option A: Using Visual Studio (Recommended)**

1. **Right-click on the `Resources` folder** (already created)
2. **Add → New Item → Resources File**
3. **Name it:** `SharedResource.en-US.resx` (English)
4. **Repeat:** `SharedResource.ar-SA.resx` (Arabic)

### **Option B: Manually Create XML Files**

If VS doesn't have the template, create these files manually:

**File:** `Resources/SharedResource.en-US.resx`
```xml
<?xml version="1.0" encoding="utf-8"?>
<root>
  <xsd:schema id="root" xmlns="" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:msdata="urn:schemas-microsoft-com:xml-msdata">
    <xsd:import namespace="http://www.w3.org/XML/1998/namespace" />
    <xsd:element name="root" msdata:IsDataSet="true">
      <xsd:complexType>
        <xsd:choice maxOccurs="unbounded">
          <xsd:element name="metadata">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name="value" type="xsd:string" minOccurs="0" />
              </xsd:sequence>
              <xsd:attribute name="name" use="required" type="xsd:string" />
              <xsd:attribute name="type" type="xsd:string" />
              <xsd:attribute name="mimetype" type="xsd:string" />
              <xsd:attribute ref="xml:space" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name="assembly">
            <xsd:complexType>
              <xsd:attribute name="alias" type="xsd:string" />
              <xsd:attribute name="name" type="xsd:string" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name="data">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name="value" type="xsd:string" minOccurs="0" msdata:Ordinal="1" />
                <xsd:element name="comment" type="xsd:string" minOccurs="0" msdata:Ordinal="2" />
              </xsd:sequence>
              <xsd:attribute name="name" type="xsd:string" use="required" msdata:Ordinal="1" />
              <xsd:attribute name="type" type="xsd:string" msdata:Ordinal="3" />
              <xsd:attribute name="mimetype" type="xsd:string" msdata:Ordinal="4" />
              <xsd:attribute ref="xml:space" msdata:Ordinal="5" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name="resheader">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name="value" type="xsd:string" minOccurs="0" msdata:Ordinal="1" />
              </xsd:sequence>
              <xsd:attribute name="name" type="xsd:string" use="required" />
            </xsd:complexType>
          </xsd:element>
        </xsd:choice>
      </xsd:complexType>
    </xsd:element>
  </xsd:schema>
  <resheader name="resmimetype">
    <value>text/microsoft-resx</value>
  </resheader>
  <resheader name="version">
    <value>2.0</value>
  </resheader>
  <resheader name="reader">
    <value>System.Resources.ResXResourceReader, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
  <resheader name="writer">
    <value>System.Resources.ResXResourceWriter, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
  
  <!-- ==================== NAVIGATION ==================== -->
  <data name="Home" xml:space="preserve">
    <value>Home</value>
  </data>
  <data name="SupportTickets" xml:space="preserve">
    <value>Support Tickets</value>
  </data>
  <data name="CreateTicket" xml:space="preserve">
    <value>Create Ticket</value>
  </data>
  
  <!-- ==================== CREATE FORM ==================== -->
  <data name="CreateNewTicket" xml:space="preserve">
    <value>Create Support Ticket</value>
  </data>
  <data name="Category" xml:space="preserve">
    <value>Category</value>
  </data>
  <data name="Priority" xml:space="preserve">
    <value>Priority</value>
  </data>
  <data name="Title" xml:space="preserve">
    <value>Ticket Title</value>
  </data>
  <data name="Description" xml:space="preserve">
    <value>Detailed Description</value>
  </data>
  <data name="BookingReference" xml:space="preserve">
    <value>Booking Reference ID</value>
  </data>
  <data name="YourName" xml:space="preserve">
    <value>Your Full Name</value>
  </data>
  <data name="Submit" xml:space="preserve">
    <value>Submit Ticket</value>
  </data>
  <data name="Cancel" xml:space="preserve">
    <value>Cancel</value>
  </data>
  
  <!-- ==================== STATUS & PRIORITY ==================== -->
  <data name="StatusOpen" xml:space="preserve">
    <value>Open</value>
  </data>
  <data name="StatusInProgress" xml:space="preserve">
    <value>In Progress</value>
  </data>
  <data name="StatusResolved" xml:space="preserve">
    <value>Resolved</value>
  </data>
  <data name="StatusClosed" xml:space="preserve">
    <value>Closed</value>
  </data>
  <data name="PriorityLow" xml:space="preserve">
    <value>Low</value>
  </data>
  <data name="PriorityMedium" xml:space="preserve">
    <value>Medium</value>
  </data>
  <data name="PriorityHigh" xml:space="preserve">
    <value>High</value>
  </data>
</root>
```

**File:** `Resources/SharedResource.ar-SA.resx`
```xml
<?xml version="1.0" encoding="utf-8"?>
<root>
  <!-- Same schema as above, only change the values -->
  
  <!-- ==================== NAVIGATION ==================== -->
  <data name="Home" xml:space="preserve">
    <value>الرئيسية</value>
  </data>
  <data name="SupportTickets" xml:space="preserve">
    <value>تذاكر الدعم</value>
  </data>
  <data name="CreateTicket" xml:space="preserve">
    <value>إنشاء تذكرة</value>
  </data>
  
  <!-- ==================== CREATE FORM ==================== -->
  <data name="CreateNewTicket" xml:space="preserve">
    <value>إنشاء تذكرة دعم</value>
  </data>
  <data name="Category" xml:space="preserve">
    <value>الفئة</value>
  </data>
  <data name="Priority" xml:space="preserve">
    <value>الأولوية</value>
  </data>
  <data name="Title" xml:space="preserve">
    <value>عنوان التذكرة</value>
  </data>
  <data name="Description" xml:space="preserve">
    <value>الوصف التفصيلي</value>
  </data>
  <data name="BookingReference" xml:space="preserve">
    <value>رقم الحجز المرجعي</value>
  </data>
  <data name="YourName" xml:space="preserve">
    <value>الاسم الكامل</value>
  </data>
  <data name="Submit" xml:space="preserve">
    <value>إرسال التذكرة</value>
  </data>
  <data name="Cancel" xml:space="preserve">
    <value>إلغاء</value>
  </data>
  
  <!-- ==================== STATUS & PRIORITY ==================== -->
  <data name="StatusOpen" xml:space="preserve">
    <value>مفتوح</value>
  </data>
  <data name="StatusInProgress" xml:space="preserve">
    <value>قيد المعالجة</value>
  </data>
  <data name="StatusResolved" xml:space="preserve">
    <value>تم الحل</value>
  </data>
  <data name="StatusClosed" xml:space="preserve">
    <value>مغلق</value>
  </data>
  <data name="PriorityLow" xml:space="preserve">
    <value>منخفضة</value>
  </data>
  <data name="PriorityMedium" xml:space="preserve">
    <value>متوسطة</value>
  </data>
  <data name="PriorityHigh" xml:space="preserve">
    <value>عالية</value>
  </data>
</root>
```

---

## **STEP 2: Using Localization in Razor Pages**

### **In Any .cshtml File:**

```cshtml
@using Microsoft.AspNetCore.Mvc.Localization
@inject IViewLocalizer Localizer

<!-- Use it anywhere in the page -->
<h1>@Localizer["CreateNewTicket"]</h1>
<label>@Localizer["Category"]</label>
<button>@Localizer["Submit"]</button>
```

### **Example: Update Create.cshtml**

Replace hardcoded text:
```cshtml
<!-- Before -->
<h1>Create Support Ticket</h1>

<!-- After -->
@inject IViewLocalizer Localizer
<h1>@Localizer["CreateNewTicket"]</h1>
```

---

## **STEP 3: Test the Localization**

### **1. Stop and Restart the App:**
```bash
# Stop
taskkill /F /IM dotnet.exe

# Run
dotnet run
```

### **2. Test Language Switching:**
1. Go to http://localhost:5080
2. Click "عربي" button in navbar
3. **Observe:**
   - Text direction changes to RTL
   - Bootstrap RTL loads
   - Cairo font loads
   - All `@Localizer` keys show Arabic text
4. Click "English" button
   - Switches back to LTR
   - Standard Bootstrap loads

---

## **📊 How It Works**

### **Flow Diagram:**
```
User clicks "عربي" button
↓
/SetLanguage?culture=ar-SA
↓
Sets Cookie: .AspNetCore.Culture=c=ar-SA|uic=ar-SA
↓
Redirects back to page
↓
_Layout.cshtml reads culture
↓
IF Arabic:
  - <html dir="rtl">
  - Load Bootstrap RTL
  - Load Cairo font
ELSE:
  - <html dir="ltr">
  - Load standard Bootstrap
↓
@Localizer["Key"] reads SharedResource.ar-SA.resx
↓
Renders Arabic text
```

---

## **🎯 Key Points**

### **1. Shared Resources = One Set of Files**
- ✅ Create `SharedResource.en-US.resx` and `SharedResource.ar-SA.resx` ONCE
- ✅ Use them in ALL pages
- ❌ Don't create separate .resx files for each page

### **2. Keys Must Match**
Both `.resx` files must have the SAME keys:
```xml
<!-- en-US -->
<data name="CreateTicket">
  <value>Create Ticket</value>
</data>

<!-- ar-SA -->
<data name="CreateTicket">
  <value>إنشاء تذكرة</value>
</data>
```

###3. **Cookie Persistence**
- Cookie expires in 1 year
- User's language preference is remembered
- Works across sessions

---

## **📁 Final File Structure**

```
MabatSupportSystem/
├── Resources/
│   ├── SharedResource.cs          ✅ Already created
│   ├── SharedResource.en-US.resx  ⚠️ You need to create
│   └── SharedResource.ar-SA.resx  ⚠️ You need to create
├── Pages/
│   ├── SetLanguage.cshtml         ✅ Already created
│   ├── SetLanguage.cshtml.cs      ✅ Already created
│   └── Shared/
│       ├── _Layout.cshtml         ✅ Already updated
│       └── _SelectLanguagePartial.cshtml ✅ Already created
└── Program.cs                     ✅ Already configured
```

---

## **🚀 Quick Start Commands**

```bash
# 1. Create the .resx files (copy from examples above)
# 2. Stop the app
taskkill /F /IM dotnet.exe

# 3. Build
dotnet build

# 4. Run
dotnet run

# 5. Test
# Navigate to http://localhost:5080
# Click "عربي" - should see RTL layout with Arabic text
```

---

## **💡 Pro Tips**

1. **Use Descriptive Keys:** `CreateNewTicket` is better than `Label1`
2. **Group Related Keys:** Use prefixes like `Nav_`, `Form_`, `Status_`
3. **Test Both Languages:** Always verify both EN and AR render correctly
4. **RTL Testing:** Check that margins, padding, icons all flip properly
5. **Font Loading:** Cairo font loads from Google Fonts - requires internet

---

## **🐛 Troubleshooting**

### **Issue: Text Not Translating**
**Solution:**
1. Check that key names match exactly (case-sensitive)
2. Verify `.resx` files are in `Resources` folder
3. Ensure `SharedResource.cs` namespace matches
4. Rebuild the project

### **Issue: RTL Not Working**
**Solution:**
1. Check browser console for CSS loading errors
2. Verify `_Layout.cshtml` has the `@if (isArabic)` logic
3. Clear browser cache
4. Check that `<html dir="rtl">` is being set

### **Issue: Cookie Not Persisting**
**Solution:**
1. Check browser allows cookies
2. Verify `SetLanguage.cshtml.cs` sets cookie correctly
3. Check cookie in browser DevTools (Application → Cookies)

---

## **✅ Testing Checklist**

- [ ] Resource files created with matching keys
- [ ] English text displays correctly
- [ ] Arabic text displays correctly
- [ ] RTL layout works (content flows right-to-left)
- [ ] Bootstrap RTL loads for Arabic
- [ ] Cairo font loads for Arabic
- [ ] Language switcher buttons work
- [ ] Cookie persists after page refresh
- [ ] All pages use `@Localizer` instead of hardcoded text

---

## **🎨 Arabic Typography Best Practices**

**Good Arabic Fonts:**
- Cairo (already configured)
- Tajawal
- Almarai
- IBM Plex Sans Arabic

**To change font, edit _Layout.cshtml:**
```html
<link href="https://fonts.googleapis.com/css2?family=Tajawal:wght@300;400;600;700&display=swap" rel="stylesheet">
<style>
    body {
        font-family: 'Tajawal', sans-serif !important;
    }
</style>
```

---

## **📞 Need More Keys?**

Add more translations to BOTH `.resx` files:

**English (`SharedResource.en-US.resx`):**
```xml
<data name="Welcome" xml:space="preserve">
  <value>Welcome to Mabat Support</value>
</data>
```

**Arabic (`SharedResource.ar-SA.resx`):**
```xml
<data name="Welcome" xml:space="preserve">
  <value>مرحباً بك في دعم مبات</value>
</data>
```

**Use in Razor:**
```cshtml
<h1>@Localizer["Welcome"]</h1>
```

---

**🎉 You now have a fully bilingual (EN/AR) application with RTL support!**
