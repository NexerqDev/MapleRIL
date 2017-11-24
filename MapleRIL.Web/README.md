# MapleRIL.Web

## Dev instructions

Ensure node.js installed.

```
$ cd [into MapleRIL.Web]
$ npm install -g webpack
$ npm install
$ npm run build-prod # for prod (use build-dev for dev)
$ cp config.json.template config.json # then edit whatever

# edit config.json
# fire up VS & MapleRIL.sln, compile the Web project and launch - .Self to use nancy's selfhost, .Asp to use ASP.NET/IIS
```