using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using u4 = System.UInt32;
using u2 = System.UInt16;
using u1 = System.Char;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;


namespace K3yManager.Works
{
    class DexWorks
    {
        private Eenclass enc = new Eenclass();
         
        private string[] header_real = new string[23]; 

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct header_item
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] magic  ;
            public uint checksum;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] siganature;
            public uint file_size;
            public uint header_size;
            public uint endian_tag;
            public uint link_size;
            public uint link_off;
            public uint map_off;
            public uint string_ids_size;
            public uint string_ids_off;
            public uint type_ids_size;
            public uint type_ids_off;
            public uint proto_ids_size;
            public uint proto_ids_off;
            public uint field_ids_size;
            public uint field_ids_off; 
            public uint method_ids_size;
            public uint method_ids_off;
            public uint class_defs_size;
            public uint class_defs_off;
            public uint data_size;
            public uint data_off;
        }

        public struct encode_field
        {
            // how to generate uleb128 use byte[]?
            public uint filed_idx_diff;
            public uint access_flag   ;
        }

        public struct encode_method
        {
            public uint  method_idx_diff;
            public uint  access_flag;
            public uint code_off;
            
        } 
        public struct class_data_header
        {
            public uint size;
            public class_data_item[] items ; 

        }
        public struct class_data_item
        {
            public uint static_size;
            public uint instance_size;
            public uint direct_size;
            public uint virtual_size;
            public encode_field[] static_field ;
            public encode_field[] instance_field;
            public encode_method[] direct_method; 
            public encode_method[] virtual_method;
        }

        public struct code_item_header
        {
            public uint size ;
            public code_item[] items ; 

        }
        public struct code_item
        {
            public ushort register_size;
            public ushort arg_size;
            public ushort out_size;
            public ushort tries_size;
            public uint debug_off;
            public uint insns_size;
            public ushort[] insns;
            /*
            public ushort padding
            public try_items[] tries
            public encode_catch_handler handler*/
        }
        public struct maplist
        {
           public uint size;
           public map_item[] list; 
        }

        public struct map_item
        {
          public  ushort type;
          public  ushort unuse;
          public  uint size;
          public  uint offset; 
        }


        public struct string_ids_item
        {
            public uint size;
            public strcontent[] strlist;  
        } 

        public struct strcontent
        {
            public uint offset; 
            public uint len;
            public string content; 
        }

        public struct method_ids_item
        {
            public uint size;
            public method_item[] methodlist;
        }

        public struct method_item
        {
            public ushort class_idx;
            public ushort proto_idx;
            public uint string_idx;
        }
        public struct Class_def_header
        {
            public uint size;
            public Class_def_Item[] class_item;
        }

        public struct Class_def_Item
        {
            public uint class_idx;
            public uint access_flags;
            public uint superclass_dix;
            public uint interfaces_off;
            public uint source_file_idx;
            public uint annotations_off;
            public uint class_data_off; 
            public uint static_value_off; 

        }

        
        private byte[] src  ; 
        private string path ;
        object structType = null;
        private header_item head;
        public maplist Maplist;
        public string_ids_item String_item;
        public method_ids_item method;
        public Class_def_header class_defs;
        public class_data_header class_data; 
        
        public DexWorks(string path)
        {
            if( path == null )
            {
                return; 
            }
            this.path = path;
            FileStream fs = new FileStream(path, FileMode.Open);
            src= new byte[fs.Length];
            fs.Read(src, 0, src.Length);
            fs.Close(); 
        }

        public void AnaMapoff()
        {
            int size = src[head.map_off]; 
           
            Maplist = new maplist();
            Maplist.size = (uint)size; 
            Maplist.list = new map_item[size];
            for (int i = 0; i < size; ++i)
            {
                structType = Maplist.list[i]; 
                ByteArrayToStructure(src, ref structType, (int)head.map_off + 4+i*12);
                Maplist.list[i] = (map_item)structType;
            }
        }

        public void AnaCode_off()
        {

        }

        public void AnaClass_data()
        {
            class_data = new class_data_header();
            class_data.size = class_defs.size;
            class_data.items = new class_data_item[class_data.size];  
            for(int i=0; i <class_data.size; ++i)
            {
                uint offset = class_defs.class_item[i].class_data_off;
                offset=Anadata_size(i , offset);

                class_data.items[i].direct_method = new encode_method[class_data.items[i].direct_size];
                class_data.items[i].virtual_method= new encode_method[class_data.items[i].virtual_size];
                class_data.items[i].instance_field = new encode_field[class_data.items[i].instance_size];
                class_data.items[i].static_field= new encode_field[class_data.items[i].direct_size];


                for (int x =0; x< class_data.items[i].static_size; ++x)
                {
                    uint dd = src[offset];
                    offset=calcleb128(offset, ref dd);

                    class_data.items[i].static_field[x].filed_idx_diff = dd;
                    uint flags = src[offset];
                    offset = calcleb128(offset, ref flags);
                    class_data.items[i].static_field[x].access_flag = flags; 
                }
                for (int x = 0; x < class_data.items[i].instance_size; ++x)
                {
                    uint dd = src[offset];
                    offset = calcleb128(offset, ref dd);

                    class_data.items[i].instance_field[x].filed_idx_diff = dd;
                    uint flags = src[offset];
                    offset = calcleb128(offset, ref flags);
                    class_data.items[i].instance_field[x].access_flag = flags;
                }
                for (int x = 0; x < class_data.items[i].direct_size; ++x)
                {
                    uint dd = src[offset];
                    offset = calcleb128(offset, ref dd);

                    class_data.items[i].direct_method[x].method_idx_diff = dd;
                    uint flags = src[offset];
                    offset = calcleb128(offset, ref flags);
                    class_data.items[i].direct_method[x].access_flag = flags;
                    uint code_off = src[offset];
                    offset = calcleb128(offset, ref code_off);
                    class_data.items[i].direct_method[x].code_off = code_off; 
                }
                for (int x = 0; x < class_data.items[i].virtual_size; ++x)
                {
                    uint dd = src[offset];
                    offset = calcleb128(offset, ref dd);

                    class_data.items[i].virtual_method[x].method_idx_diff = dd;
                    uint flags = src[offset];
                    offset = calcleb128(offset, ref flags);
                    class_data.items[i].virtual_method[x].access_flag = flags;
                    uint code_off = src[offset];
                    offset = calcleb128(offset, ref code_off);
                    class_data.items[i].virtual_method[x].code_off = code_off;
                }

            }
        }
        private uint calcleb128(uint offset , ref uint  dd)
        {
            if (((dd & 0xff) > 0x7f))
            {
                if ((dd & 0xff00) > 0x7f00)
                {
                    if ((dd &0xff0000)>0x7f0000)
                    {
                        if((dd &0xff000000)> 0x7f000000)
                        {
                            offset += 5; 
                        }
                        offset += 4;
                        dd = (dd & 0x7f) | ((dd & 0xff00) >> 1) | ((dd & 0xff0000) >> 1) | ((dd & 0xff000000) >> 1); 
                    }
                    offset += 3;
                    dd = (dd & 0x7f) | ((dd & 0xff00) >> 1) | ((dd & 0xff0000) >> 1) ; 
                }
                offset += 2;
                // 2zijie 
                dd = (dd & 0x7f) | ((dd & 0xff00) >> 1); 
            }

            offset += 1; 

            return offset; 
        }
        private uint  Anadata_size(int i ,uint offset)
        {
            uint tmpoffset = 0; 
            uint usize = 0; 
            uint static_size = src[offset]; 
            if((static_size & 0xff) > 0x7f)
            {
                static_size &= 0xffff;
                usize = 1; 
            }
            tmpoffset = offset + 1 + usize; 
            uint instance_size = src[tmpoffset]; 
            if((instance_size &0xff) >0x7f)
            {
                instance_size &= 0xffff;
                usize = 1; 
            }
            tmpoffset = tmpoffset + 1 + usize;
            uint direct_size = src[tmpoffset]; 
            if((direct_size&0xff) >0x7f)
            {
                direct_size &= 0xffff;
                usize = 1; 
            }
            tmpoffset = tmpoffset + 1 + usize; 
            uint virtual_size = src[tmpoffset];
            if((virtual_size &0xff) >0x7f)
            {
                virtual_size &= 0xffff;
                usize = 1; 
            }

            tmpoffset = tmpoffset + 1 + usize; 

            class_data.items[i].instance_size = instance_size;
            class_data.items[i].static_size = static_size;
            class_data.items[i].virtual_size = virtual_size;
            class_data.items[i].direct_size = direct_size;

            return tmpoffset; 
        }


        private int checkuleb128(byte src)
        {
            //TODO checksize ;
            return 0;
        }
        public void AnaClass_defs()
        {
            uint offset = 0;
            uint size = 0;
            findoffandsize(ref offset, ref size, 0x0006);
            class_defs = new Class_def_header();
            class_defs.size = size;
            class_defs.class_item = new Class_def_Item[size];
            for (int i = 0; i < size; ++i)
            {
                structType = class_defs.class_item[i];
                ByteArrayToStructure(src, ref structType, (int)head.class_defs_off + i * 28);
                class_defs.class_item[i] = (Class_def_Item)structType ;
            }
        }
        
        private void findoffandsize(ref uint offset , ref uint size,uint type)
        {
            for (int i = 0; i < Maplist.size; i++)
            {
                if (Maplist.list[i].type == type)
                {
                    offset = Maplist.list[i].offset;
                    size = Maplist.list[i].size;
                }
            }
        }

        private void AnaProtooff()
        { }

        private void AnaFieldoff()
        { }

        public void AnaTypeoff()
        {

        }

        public void AnaMethodoff()
        {
            uint offset = 0;
            uint size = 0;
            findoffandsize( ref offset,ref size, 0x0005);
            method = new method_ids_item();
            method.size = size;
            method.methodlist = new method_item[size];
            for (int i = 0; i < size; ++i)
            {
                structType = method.methodlist[i];
                ByteArrayToStructure(src, ref structType, (int)head.method_ids_off  + i * 8);
                method.methodlist[i] = (method_item)structType;
            }

        }
        public void AnaStringoff()
        {
            uint offset = 0;
            uint items = 0;
            findoffandsize(ref offset, ref items, 0x0001);
            String_item = new string_ids_item();
            String_item.size = items; 
            String_item.strlist = new strcontent[items]; 
            for (int i = 0; i <items; i++)
            {
                uint string_off = (uint)src[offset + i * 4];
                uint size = (uint)src[string_off] & 0xff; 
                if(size > 0x7f)
                {
                    MessageBox.Show("Size is bigger than 0x7F"); 
                    //deal size ;                    
                }
                else
                {
                    byte[] xx = new byte[size]; 
                    for(int j=0; j <size; ++j)
                    {
                        xx[j] = src[string_off + 1];
                    }
                    String_item.strlist[i].content = Encoding.UTF8.GetString(xx);
                }
                String_item.strlist[i].len = size;
                String_item.strlist[i].offset = offset; 
            }

        }


        public string[] AnaHeader()
        {
            
            string[] result = new string[23];
            head = new header_item();
            structType = head; 
            ByteArrayToStructure(src, ref structType, 0);
            head = (header_item)structType;
            if(head.magic[0]!=0x64)
            {
                return null; 
            }
            result[0] = header_real[0] = Encoding.Default.GetString(head.magic);
            result[1] = header_real[1] = Convert.ToString(head.checksum,16);
            result[2] = header_real[2] = enc.hex2str(head.siganature); 
            result[3] = header_real[3] = Convert.ToString(head.file_size,16);
            result[4] = header_real[4] = Convert.ToString(head.header_size,16);
            result[5] = header_real[5] = Convert.ToString(head.endian_tag,16);
            result[6] = header_real[6] = Convert.ToString(head.link_size,16);
            result[7] = header_real[7] = Convert.ToString(head.link_off,16);
            result[8] = header_real[8] = Convert.ToString(head.map_off,16);
            result[9] = header_real[9] = Convert.ToString(head.string_ids_size,16);
            result[10] = header_real[10] = Convert.ToString(head.string_ids_off,16);
            result[11] = header_real[11] = Convert.ToString(head.type_ids_size,16);
            result[12] = header_real[12] = Convert.ToString(head.type_ids_off,16);
            result[13] = header_real[13] = Convert.ToString(head.proto_ids_size,16);
            result[14] = header_real[14] = Convert.ToString(head.proto_ids_off,16);
            result[15] = header_real[15] = Convert.ToString(head.field_ids_size,16);
            result[16] = header_real[16] = Convert.ToString(head.field_ids_off,16);
            result[17] = header_real[17] = Convert.ToString(head.method_ids_size,16);
            result[18] = header_real[18] = Convert.ToString(head.method_ids_off,16);
            result[19] = header_real[19] = Convert.ToString(head.class_defs_size,16);
            result[20] = header_real[20] = Convert.ToString(head.class_defs_off,16);
            result[21] = header_real[21] = Convert.ToString(head.data_size,16);
            result[22] = header_real[22] = Convert.ToString(head.data_off,16);
            return result; 
        }


        byte[] StructureToByteArray(object obj)
        {
            int len = Marshal.SizeOf(obj);
            byte[] arr = new byte[len];
            IntPtr ptr = Marshal.AllocHGlobal(len);
            Marshal.StructureToPtr(obj, ptr, true);
            Marshal.Copy(ptr, arr, 0, len);
            Marshal.FreeHGlobal(ptr);
            return arr; 

        }

         public static void ByteArrayToStructure(byte[] bytearray, ref object obj, int startoffset)
        {
            int len = Marshal.SizeOf(obj);
            IntPtr i = Marshal.AllocHGlobal(len);
          //  obj = Marshal.PtrToStructure(i, obj.GetType());
            try
            {
                Marshal.Copy(bytearray, startoffset, i, len);
            }
            catch(Exception)
            {  }

            obj = Marshal.PtrToStructure(i, obj.GetType());
            Marshal.FreeHGlobal(i); 

        }

    }



    /*
    DEX_INLINE int readUnsignedLeb128(const u1** pStream) {
    const u1* ptr = *pStream;
    int result = *(ptr++);
    if (result > 0x7f) {
        int cur = *(ptr++);
        result = (result & 0x7f) | ((cur & 0x7f) << 7);
        if (cur > 0x7f) {
            cur = *(ptr++);
            result |= (cur & 0x7f) << 14;
            if (cur > 0x7f) {
                cur = *(ptr++);
                result |= (cur & 0x7f) << 21;
                if (cur > 0x7f) {
              
                    cur = *(ptr++);
                    result |= cur << 28;
                }
            }
        }
    }

    * pStream = ptr;
    return result;
}

    */
}
