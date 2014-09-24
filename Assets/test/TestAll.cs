﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

public class TestAll {
    public void Run () {
        LoadProto ();

		// different ways to create SpObject
		CheckObj (CreateObject ());
		CheckObj (CreateObject2 ());
		SpObject obj = CreateObject3 ();
		CheckObj (obj);
        Util.DumpObject (obj);

        MemoryStream encode_stream = new MemoryStream ();
        MemoryStream decode_stream = new MemoryStream ();
        MemoryStream pack_stream = new MemoryStream ();

        SpCodec.Encode ("foobar", obj, encode_stream);

        encode_stream.Position = 0;
        Util.DumpStream (encode_stream);

        encode_stream.Position = 0;
        SpPacker.Pack (encode_stream, pack_stream);

        pack_stream.Position = 0;
        Util.DumpStream (pack_stream);

        pack_stream.Position = 0;
        SpPacker.Unpack (pack_stream, decode_stream);

        decode_stream.Position = 0;
        Util.DumpStream (decode_stream);

        decode_stream.Position = 0;
		SpObject newObj = SpCodec.Decode ("foobar", decode_stream);
		CheckObj (newObj);
        Util.DumpObject (newObj);
	}
	
	private void LoadProto () {
		string path = Util.GetFullPath("foobar.sproto");
		using (FileStream stream = new FileStream (path, FileMode.Open)) {
			SpTypeManager.Import (stream);
		}
	}

    private SpObject CreateObject () {
        SpObject obj = new SpObject ();

		obj.Insert ("a", new SpObject ("hello"));
        obj.Insert ("b", new SpObject (1000000));
        obj.Insert ("c", new SpObject (true));

        SpObject d = new SpObject ();
        d.Insert ("a", "world");
        d.Insert ("c", -1);
        obj.Insert ("d", d);

        SpObject e = new SpObject ();
        e.Append ("ABC");
        e.Append ("def");
        obj.Insert ("e", e);

        SpObject f = new SpObject ();
        f.Append (-3);
        f.Append (-2);
        f.Append (-1);
        f.Append (0);
        f.Append (1);
        f.Append (2);
        obj.Insert ("f", f);

        SpObject g = new SpObject ();
        g.Append (true);
        g.Append (false);
        g.Append (true);
        obj.Insert ("g", g);

        SpObject h = new SpObject ();
        {
            SpObject t = new SpObject ();
            t.Insert ("b", 100);
            h.Append (t);
        }
        {
            SpObject t = new SpObject ();
            h.Append (t);
        }
        {
            SpObject t = new SpObject ();
            t.Insert ("b", -100);
            t.Insert ("c", false);
            h.Append (t);
        }
        {
            SpObject t = new SpObject ();
            t.Insert ("b", 0);

            SpObject he = new SpObject ();
            he.Append ("test");
            t.Insert ("e", he);
            h.Append (t);
        }
        obj.Insert ("h", h);
        
        return obj;
    }

    private SpObject CreateObject2 () {
        SpObject obj = new SpObject ();

        obj.Insert ("a", "hello");
        obj.Insert ("b", 1000000);
        obj.Insert ("c", true);

        SpObject d = new SpObject ();
        d.Insert ("a", "world");
        d.Insert ("c", -1);
        obj.Insert ("d", d);

		obj.Insert ("e", new SpObject (SpObject.ArgType.Array, "ABC", "def"));
		obj.Insert ("f", new SpObject (SpObject.ArgType.Array, -3, -2, -1, 0, 1, 2));
		obj.Insert ("g", new SpObject (SpObject.ArgType.Array, true, false, true));

        SpObject h = new SpObject ();
        {
            SpObject t = new SpObject ();
            t.Insert ("b", 100);
            h.Append (t);
        }
        {
            SpObject t = new SpObject ();
            h.Append (t);
        }
        {
            SpObject t = new SpObject ();
            t.Insert ("b", -100);
            t.Insert ("c", false);
            h.Append (t);
        }
        {
            SpObject t = new SpObject ();
            t.Insert ("b", 0);

            SpObject he = new SpObject ();
            he.Append ("test");
            t.Insert ("e", he);
            h.Append (t);
        }
        obj.Insert ("h", h);

        return obj;
    }

	private SpObject CreateObject3 () {
		SpObject obj = new SpObject (SpObject.ArgType.Table, 
		                          	 "a", "hello",
		                             "b", 1000000,
		                             "c", true,
		                             "d", new SpObject (SpObject.ArgType.Table,
		                 								"a", "world",
		                 								"c", -1),
		                             "e", new SpObject (SpObject.ArgType.Array, "ABC", "def"),
		                             "f", new SpObject (SpObject.ArgType.Array, -3, -2, -1, 0, 1, 2),
		                             "g", new SpObject (SpObject.ArgType.Array, true, false, true),
		                             "h", new SpObject (SpObject.ArgType.Array,
										                 new SpObject (SpObject.ArgType.Table, "b", 100),
										                 new SpObject (),
										                 new SpObject (SpObject.ArgType.Table, "b", -100, "c", false),
										                 new SpObject (SpObject.ArgType.Table, "b", 0, "e", new SpObject (SpObject.ArgType.Array, "test")))
		                   );

		return obj;
	}

	private void CheckObj (SpObject obj) {
		Util.Assert (obj["a"].AsString ().Equals ("hello"));
		Util.Assert (obj["b"].AsInt () == 1000000);
		Util.Assert (obj["c"].AsBoolean () == true);
		Util.Assert (obj["d"]["a"].AsString ().Equals ("world"));
		Util.Assert (obj["d"]["c"].AsInt () == -1);
		Util.Assert (obj["e"][0].AsString ().Equals ("ABC"));
		Util.Assert (obj["e"][1].AsString ().Equals ("def"));
		Util.Assert (obj["f"][0].AsInt () == -3);
		Util.Assert (obj["f"][1].AsInt () == -2);
		Util.Assert (obj["f"][2].AsInt () == -1);
		Util.Assert (obj["f"][3].AsInt () == 0);
		Util.Assert (obj["f"][4].AsInt () == 1);
		Util.Assert (obj["f"][5].AsInt () == 2);
		Util.Assert (obj["g"][0].AsBoolean () == true);
		Util.Assert (obj["g"][1].AsBoolean () == false);
		Util.Assert (obj["g"][2].AsBoolean () == true);
		Util.Assert (obj["h"][0]["b"].AsInt () == 100);
		Util.Assert (obj["h"][1].Value == null);
		Util.Assert (obj["h"][2]["b"].AsInt () == -100);
		Util.Assert (obj["h"][2]["c"].AsBoolean () == false);
		Util.Assert (obj["h"][3]["b"].AsInt () == 0);
		Util.Assert (obj["h"][3]["e"][0].AsString ().Equals ("test"));
	}
}

