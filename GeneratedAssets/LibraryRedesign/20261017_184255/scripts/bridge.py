import socket,json,sys
from pathlib import Path
p=Path(sys.argv[1]).resolve()
code="__file__="+repr(str(p))+"\n"+p.read_text(encoding="utf-8")
s=socket.create_connection(("127.0.0.1",9876),10);s.settimeout(600)
s.sendall(json.dumps({"type":"execute_code","params":{"code":code}}).encode());b=b""
while True:
 part=s.recv(1048576)
 if not part:raise RuntimeError("Blender closed connection")
 b+=part
 try:r=json.loads(b);break
 except json.JSONDecodeError:pass
s.close();print(json.dumps(r,ensure_ascii=False))
if r.get("status")!="success" or r.get("result",{}).get("executed") is False:sys.exit(1)
