﻿using System.Collections;
using System.IO;

public class TestUnit {

	SpTypeManager manager;

	private void TestStr (string s) {
		SpObject obj = new SpObject (SpObject.ArgType.Table, "a", s);
		
		Util.Log ("------------------TestStr----------------------------");
		Util.Log (s);
		
		Util.Log ("Encode");
		SpStream encode_stream = new SpStream ();
		manager.Codec.Encode ("ss", obj, encode_stream);
		encode_stream.Position = 0;
		Util.DumpStream (encode_stream);

		Util.Log ("Pack");
		encode_stream.Position = 0;
		SpStream pack_stream = new SpStream ();
		SpPacker.Pack (encode_stream, pack_stream);
		pack_stream.Position = 0;
		Util.DumpStream (pack_stream);
		
		Util.Log ("Unpack");
		pack_stream.Position = 0;
		SpStream unpack_stream = new SpStream ();
		SpPacker.Unpack (pack_stream, unpack_stream);
		unpack_stream.Position = 0;
		Util.DumpStream (unpack_stream);
		
		Util.Log ("Decode");
		unpack_stream.Position = 0;
		SpObject dobj = manager.Codec.Decode ("ss", unpack_stream);
		string ds = dobj["a"].AsString ();
		Util.Log (ds);
		Util.Assert (s == ds);
	}

	public void Run () {
		
		string proto = @"
			.ss {
				a 0 : string
			}
		";
		manager = SpTypeManager.Import (proto);
		
		TestStr ("");
		TestStr ("123");
		TestStr ("123456");
		TestStr ("12345678");
		TestStr ("12345678123");
		TestStr ("12345678123456");
		TestStr ("1234567812345678");
		TestStr ("12345678123456781234567812345678");
		TestStr ("123456781234567812345678123456781");
		TestStr ("123456781234567812345678123456781234567");
	}
	
	private void CheckObj (SpObject obj) {
		Util.DumpObject (obj);
        Util.Assert (obj["person"][10000]["id"].AsInt () == 10000);
        Util.Assert (obj["person"][10000]["name"].AsString ().Equals ("Alice"));
        Util.Assert (obj["person"][10000]["phone"][0]["type"].AsInt () == 1);
        Util.Assert (obj["person"][10000]["phone"][0]["number"].AsString ().Equals ("123456789"));
        Util.Assert (obj["person"][10000]["phone"][1]["type"].AsInt () == 2);
        Util.Assert (obj["person"][10000]["phone"][1]["number"].AsString ().Equals ("87654321"));
        Util.Assert (obj["person"][20000]["id"].AsInt () == 20000);
        Util.Assert (obj["person"][20000]["name"].AsString ().Equals ("Bob"));
        Util.Assert (obj["person"][20000]["phone"][0]["type"].AsInt () == 3);
        Util.Assert (obj["person"][20000]["phone"][0]["number"].AsString ().Equals ("01234567890"));
    }
}
