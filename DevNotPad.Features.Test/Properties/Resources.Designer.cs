﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DevNotPad.Features.Test.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("DevNotPad.Features.Test.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;bookstore xmlns=&quot;http://www.contoso.com/books&quot;&gt;
        ///  &lt;book genre=&quot;autobiography&quot; publicationdate=&quot;1981-03-22&quot; ISBN=&quot;1-861003-11-0&quot;&gt;
        ///    &lt;title&gt;The Autobiography of Benjamin Franklin&lt;/title&gt;
        ///    &lt;author&gt;
        ///      &lt;first-name&gt;Benjamin&lt;/first-name&gt;
        ///      &lt;last-name&gt;Franklin&lt;/last-name&gt;
        ///    &lt;/author&gt;
        ///    &lt;price&gt;8.99&lt;/price&gt;
        ///  &lt;/book&gt;
        ///  &lt;book genre=&quot;novel&quot; publicationdate=&quot;1967-11-17&quot; ISBN=&quot;0-201-63361-2&quot;&gt;
        ///    &lt;title&gt;The Confidence Man&lt;/title&gt;
        ///    &lt;author&gt;
        ///      &lt;first-name&gt;Herman&lt;/first-name&gt;
        ///      &lt;las [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string bookstore_sample {
            get {
                return ResourceManager.GetString("bookstore_sample", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot;?&gt;
        ///&lt;xs:schema attributeFormDefault=&quot;unqualified&quot; elementFormDefault=&quot;qualified&quot; targetNamespace=&quot;http://www.contoso.com/books&quot; xmlns:xs=&quot;http://www.w3.org/2001/XMLSchema&quot;&gt;
        ///  &lt;xs:element name=&quot;bookstore&quot;&gt;
        ///    &lt;xs:complexType&gt;
        ///      &lt;xs:sequence&gt;
        ///        &lt;xs:element maxOccurs=&quot;unbounded&quot; name=&quot;book&quot;&gt;
        ///          &lt;xs:complexType&gt;
        ///            &lt;xs:sequence&gt;
        ///              &lt;xs:element name=&quot;title&quot; type=&quot;xs:string&quot; /&gt;
        ///              &lt;xs:element name=&quot;author&quot;&gt;
        ///              [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string bookstore_schema {
            get {
                return ResourceManager.GetString("bookstore_schema", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot;?&gt;
        ///&lt;menu id=&quot;file&quot; value=&quot;File&quot;&gt;
        ///  &lt;popup&gt;
        ///    &lt;menuitem value=&quot;New&quot; onclick=&quot;CreateNewDoc()&quot; /&gt;
        ///    &lt;menuitem value=&quot;Open&quot; onclick=&quot;OpenDoc()&quot; /&gt;
        ///    &lt;menuitem value=&quot;Close&quot; onclick=&quot;CloseDoc()&quot; /&gt;
        ///  &lt;/popup&gt;
        ///&lt;/menu&gt;.
        /// </summary>
        internal static string simpleXml1 {
            get {
                return ResourceManager.GetString("simpleXml1", resourceCulture);
            }
        }
    }
}
