<template>
    <div id="about">
        <h1>About MapleRIL</h1>
        MapleRIL is a MapleStory cross-region item lookup &quot;search engine&quot;. It began as a Windows application and this is still available from the <a href="https://github.com/nicholastay/MapleRIL/releases">Releases section</a> of the project GitHub repository. It is now a website aimed towards ease of use.
        <br /><br />
        <h3>Author</h3>
        MapleRIL is primarily authored (and up &#39;til now has only been authored) by one person, <a href="https://twitter.com/@_nexerq">@_nexerq</a>.
        <br /><br />
        <h4>Other Technical Goodness</h4>
        <p>
            MapleRIL is copyright (c) 2017 Nicholas Tay / &quot;Nexerq&quot; &lt;nexerq[at]gmail[dot]com&gt;.<br />
            Licensed under the zlib/libpng license - the full license text can be found in the root of the repository for this project.
        </p>
        <p>
            MapleRIL also utilizes libraries from other open source projects and would not be possible without them, including:
            <ul>
                <li>MapleLib v2.0.0, copyright (c) 2009, 2010, 2015 Snow and haha01haha01</li>
            </ul>
        </p>
        <p>
            Currently loaded regions:
            <ul>
                <li v-for="r in $root.regions">{{ r.region }} ({{ r.version }})</li>
            </ul>
        </p>
    </div>
</template>