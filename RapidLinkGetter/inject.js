var panhelper = {};
let userAgent = '';

function getCookie(e) {
    let o, t;
    let n = document, c = decodeURI;
    return n.cookie.length > 0 && (o = n.cookie.indexOf(e + "="), -1 != o) ? (o = o + e.length + 1, t = n.cookie.indexOf(";", o), -1 == t && (t = n.cookie.length), c(n.cookie.substring(o, t))) : "";
}
function encode(str) {
    return btoa(unescape(encodeURIComponent(btoa(unescape(encodeURIComponent(str))))));
}

function decode(str) {
    return decodeURIComponent(escape(atob(decodeURIComponent(escape(atob(str))))));
}
function getShareType() {
    console.log(yunData);
    return yunData.SHARE_PUBLIC === 1 ? 'public' : 'secret';
}

function isSingleShare() {
    return yunData.SHAREPAGETYPE === "single_file_page";
}

function isSelfShare() {
    return yunData.MYSELF === 1;
}

function getExtra() {
    let seKey = decodeURIComponent(getCookie('BDCLND'));
    return '{' + '"sekey":"' + seKey + '"' + "}";
}

function getSelectedFile() {
    if (isSingleShare()) {
        return yunData.FILEINFO;
    } else {
        return require("disk-share:widget/pageModule/list/listInit.js").getCheckedItems();
    }
}
function getLogID() {
    let name = "BAIDUID";
    let u = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/~！@#￥%……&";
    let d = /[\uD800-\uDBFF][\uDC00-\uDFFFF]|[^\x00-\x7F]/g;
    let f = String.fromCharCode;

    function l(e) {
        if (e.length < 2) {
            let n = e.charCodeAt(0);
            return 128 > n ? e : 2048 > n ? f(192 | n >>> 6) + f(128 | 63 & n) : f(224 | n >>> 12 & 15) + f(128 | n >>> 6 & 63) + f(128 | 63 & n);
        }
        let n = 65536 + 1024 * (e.charCodeAt(0) - 55296) + (e.charCodeAt(1) - 56320);
        return f(240 | n >>> 18 & 7) + f(128 | n >>> 12 & 63) + f(128 | n >>> 6 & 63) + f(128 | 63 & n);
    }

    function g(e) {
        return (e + "" + Math.random()).replace(d, l);
    }

    function m(e) {
        let n = [0, 2, 1][e.length % 3];
        let t = e.charCodeAt(0) << 16 | (e.length > 1 ? e.charCodeAt(1) : 0) << 8 | (e.length > 2 ? e.charCodeAt(2) : 0);
        let o = [u.charAt(t >>> 18), u.charAt(t >>> 12 & 63), n >= 2 ? "=" : u.charAt(t >>> 6 & 63), n >= 1 ? "=" : u.charAt(63 & t)];
        return o.join("");
    }

    function h(e) {
        return e.replace(/[\s\S]{1,3}/g, m);
    }

    function p() {
        return h(g((new Date()).getTime()));
    }

    function w(e, n) {
        return n ? p(String(e)).replace(/[+\/]/g, (e) => {
            return "+" == e ? "-" : "_";
        }).replace(/=/g, "") : p(String(e));
    }

    return w(getCookie(name));
}

//Load PanHelper Required Content
let res = JSON.parse(decode("ZXlKaklqb3lNREFzSW5ZaU9pSTBMak11TWlJc0lta2lPbHN5TlRBMU1qZ3NOemM0TnpVd1hTd2lkU0k2SW1oMGRIQnpPaTh2ZDNkM0xtSmhhV1IxZVhWdUxuZHBhMmt2YVc1emRHRnNiQzVvZEcxc0lpd2ljeUk2SWpnd01UUTVJaXdpWmlJNk1Dd2llaUk2SW1oMGRIQnpPaTh2ZDNkM0xtSmhhV1IxZVhWdUxuZHBhMmt2WW1GcFpIVjVkVzR1ZFhObGNpNXFjeUlzSW1FaU9pSk1iMmRUZEdGMGFYTjBhV01pTENKeElqb2lhSFIwY0hNNkx5OWlaSGxqWkc0dWIzTnpMV051TFdkMVlXNW5lbWh2ZFM1aGJHbDVkVzVqY3k1amIyMHZjMk52WkdVdWNHNW5JaXdpZHlJNkltaDBkSEJ6T2k4dmQzZDNMbUpoYVdSMWVYVnVMbmRwYTJrdmVtZ3RZMjR2WVhOemFYTjBZVzUwTG1oMGJXd2lMQ0p3SWpwN0ltZ2lPaUpvZEhSd2N6b3ZMM0JoYmk1aVlXbGtkUzVqYjIwdmNtVnpkQzh5TGpBdmVIQmhiaTl0ZFd4MGFXMWxaR2xoUDIxbGRHaHZaRDFtYVd4bGJXVjBZWE1tWkd4cGJtczlNU0lzSW5NaU9pSm9kSFJ3Y3pvdkwzQmhiaTVpWVdsa2RTNWpiMjB2WVhCcEwzTm9ZWEpsWkc5M2JteHZZV1EvWTJoaGJtNWxiRDFqYUhWdWJHVnBKbU5zYVdWdWRIUjVjR1U5TVRJbWQyVmlQVEVtWVhCd1gybGtQVEkxTURVeU9DSjlMQ0p2SWpvaTVwV1o1NmlMSWl3aWFDSTZJbWgwZEhCek9pOHZkM2QzTG1KaGFXUjFlWFZ1TG5kcGEya3ZlbWd0WTI0dklpd2lkSFFpT2pNd01EQXNJblFpT25zaVlTSTZJdWl2dCtpK2srV0ZwZWFhbCtXUHQrKzhqT1dQcithSnErYVBqK1M0aXVhV3VlUzZqT2U3dE9lZ2dlV0ZqZWkwdWVpT3QrV1BsaUVpTENKaUlqb2k1WWlkNXF5aDVMMi81NVNvNksrMzZMNlQ1WVdsNXBxWDVZKzNJaXdpWXlJNkl1YWFsK1dQdCthdG8rZWhydSs4ak9hdG8rV2NxT1dJbmVXbmkrV01sdVM0cmVPQWd1T0FndU9BZ2lJc0ltUWlPaUxtbXBmbGo3Zmt1STNtcmFQbm9hN3Z2SXpvcjdma3ZiL25sS2psdnE3a3Y2SG1pYXZub0lIbGhZM290TG5vanJmbGo1WWlMQ0psSWpvaVFYSnBZZW1UdnVhT3BlaU90K1dQbHVtY2dPaW1nZW1GamVXUWlEeGhJR2h5WldZOVhDSm9kSFJ3Y3pvdkwzZDNkeTVpWVdsa2RYbDFiaTUzYVd0cEwzcG9MV051TDJGemMybHpkR0Z1ZEM1b2RHMXNYQ0lnZEdGeVoyVjBQVndpWDJKc1lXNXJYQ0krNDRDUTU3MlI1NXVZNUxpSDZJTzk1WXFwNW9tTDQ0Q1JQQzloUHVTOXYrZVVxQ0lzSW1ZaU9pTG9yN2Zsc0licGs3N21qcVhscEkzbGlMYmxpTERtbEsvbWpJRkJjbWxoNTVxRTVMaUw2TDI5NVptbzVMaXQ3N3lNNW82bzZJMlE1TDIvNTVTb0lEeGhJR2h5WldZOVhDSm9kSFJ3T2k4dmNHRnVMbUpoYVdSMWVYVnVMbmRwYTJrdlpHOTNibHdpSUhSaGNtZGxkRDFjSWw5aWJHRnVhMXdpUGxoa2IzZHVQQzloUGlJc0ltY2lPaUxuZ3JubGg3dm1qSW5wa3E3bGo1SHBnSUhwazc3bWpxWG9oN004WVNCb2NtVm1QVndpYUhSMGNITTZMeTkzZDNjdVltRnBaSFY1ZFc0dWQybHJhUzk2YUMxamJpOXRiM1J5YVhndWFIUnRiRndpSUhSaGNtZGxkRDFjSWw5aWJHRnVhMXdpUGsxdmRISnBlRHd2WVQ3dnZJem1sSy9taklIbW5LemxuTERsa296b3Y1em5xSXZrdUl2b3ZiMGlMQ0pvSWpvaTZLKzM1WVdJNWE2SjZLT0ZJRHhoSUdoeVpXWTlYQ0pvZEhSd2N6b3ZMM2QzZHk1aVlXbGtkWGwxYmk1M2FXdHBMM3BvTFdOdUwyRnpjMmx6ZEdGdWRDNW9kRzFzWENJZ2RHRnlaMlYwUFZ3aVgySnNZVzVyWENJKzQ0Q1E1NzJSNTV1WTVMaUg2SU85NVlxcDVvbUw0NENSUEM5aFBpQThZajUyTWk0ekxqSThMMkkrSU9XUWp1ZUN1ZVdIdSttVHZ1YU9wZVM0aStpOXZlKzhqT2lMcGVTNGkraTl2ZVdrc2VpMHBlKzhqT2l2dCtXOGdPV1FyeUE4WVNCb2NtVm1QVndpYUhSMGNITTZMeTkzZDNjdVltRnBaSFY1ZFc0dWQybHJhUzk2YUMxamJpOXhkV1Z6ZEdsdmJpNW9kRzFzWENJZ2RHRnlaMlYwUFZ3aVgySnNZVzVyWENJKzVhU0g1NVNvNlpPKzVvNmxQQzloUGlJc0lta2lPaUxuZ3JubGg3dnBrNzdtanFYbm03VG1qcVhrdUl2b3ZiM3Z2SXpvaTZYbmdybmxoN3Ztc3FIbGo0M2x1cFRtaUpibGg3cm5qckEwTURQcGxKbm9yNi92dkl6b3I3ZmxzSjNvcjVVZ1BHRWdhSEpsWmoxY0ltaDBkSEJ6T2k4dmQzZDNMbUpoYVdSMWVYVnVMbmRwYTJrdmVtZ3RZMjR2ZUdSdmQyNHVhSFJ0YkZ3aUlIUmhjbWRsZEQxY0lsOWliR0Z1YTF3aVBrRnlhV0hrdUl2b3ZiMDhMMkUrSU9hSWxpQThZU0JvY21WbVBWd2lhSFIwY0hNNkx5OTNkM2N1WW1GcFpIVjVkVzR1ZDJscmFTOTZhQzFqYmk5dGIzUnlhWGd1YUhSdGJGd2lJSFJoY21kbGREMWNJbDlpYkdGdWExd2lQbEpRUStTNGkraTl2VHd2WVQ0aUxDSnFJam9pNTRLNTVZZTc2Wk8rNW82bDU1dTA1bzZsNUxpTDZMMjk3N3lNNkl1bDU0SzU1WWU3NXJLaDVZK041YnFVNW9pVzVZZTY1NDZ3TkRBejZaU1o2Syt2Nzd5TTZLKzM1YkNkNksrVklEeGhJR2h5WldZOVhDSm9kSFJ3Y3pvdkwzZDNkeTVpWVdsa2RYbDFiaTUzYVd0cEwzcG9MV051TDNoa2IzZHVMbWgwYld4Y0lpQjBZWEpuWlhROVhDSmZZbXhoYm10Y0lqNUJjbWxoNUxpTDZMMjlQQzloUGlEbWlKWWdQR0VnYUhKbFpqMWNJbWgwZEhCek9pOHZkM2QzTG1KaGFXUjFlWFZ1TG5kcGEya3ZlbWd0WTI0dmJXOTBjbWw0TG1oMGJXeGNJaUIwWVhKblpYUTlYQ0pmWW14aGJtdGNJajVTVUVQa3VJdm92YjA4TDJFK0lpd2lheUk2SWtGUVNlUzRpK2k5dmUrOGlPbUFndWVVcU9TNmpqeGhJR2h5WldZOVhDSm9kSFJ3T2k4dmNHRnVMbUpoYVdSMWVYVnVMbmRwYTJrdlpHOTNibHdpSUhSaGNtZGxkRDFjSWw5aWJHRnVhMXdpUGtsRVRUd3ZZVDd2dkl3OFlTQm9jbVZtUFZ3aWFIUjBjRG92TDNCaGJpNWlZV2xrZFhsMWJpNTNhV3RwTDJSdmQyNWNJaUIwWVhKblpYUTlYQ0pmWW14aGJtdGNJajVPUkUwOEwyRSs3N3lNNXJXUDZLZUk1Wm1vNkllcTViaW01TGlMNkwyOTc3eUpQR0VnYUhKbFpqMWNJbWgwZEhCek9pOHZkM2QzTG1KaGFXUjFlWFZ1TG5kcGEya3ZlbWd0WTI0dmFXUnRMbWgwYld3ajVwV0k1cDZjNXJ5VTU2UzZYQ0lnZEdGeVoyVjBQVndpWDJKc1lXNXJYQ0krUjBsR1BDOWhQaUlzSW13aU9pSkJjbWxoNUxpTDZMMjk3N3lJNllDQzU1U281THFPUEdFZ2FISmxaajFjSW1oMGRIQTZMeTl3WVc0dVltRnBaSFY1ZFc0dWQybHJhUzlrYjNkdVhDSWdkR0Z5WjJWMFBWd2lYMkpzWVc1clhDSStXR1J2ZDI0OEwyRSs3N3lKUEdFZ2FISmxaajFjSW1oMGRIQnpPaTh2ZDNkM0xtSmhhV1IxZVhWdUxuZHBhMmt2ZW1ndFkyNHZlR1J2ZDI0dWFIUnRiQ1BtbFlqbW5wem12SlRucExwY0lpQjBZWEpuWlhROVhDSmZZbXhoYm10Y0lqNUhTVVk4TDJFK0lpd2liU0k2SWxKUVErUzRpK2k5dmUrOGlPbUFndWVVcU9TNmpqeGhJR2h5WldZOVhDSm9kSFJ3T2k4dmNHRnVMbUpoYVdSMWVYVnVMbmRwYTJrdlpHOTNibHdpSUhSaGNtZGxkRDFjSWw5aWJHRnVhMXdpUGsxdmRISnBlRHd2WVQ3dnZJdzhZU0JvY21WbVBWd2lhSFIwY0RvdkwzQmhiaTVpWVdsa2RYbDFiaTUzYVd0cEwyUnZkMjVjSWlCMFlYSm5aWFE5WENKZllteGhibXRjSWo1QmNtbGhNaUJVYjI5c2N6d3ZZVDd2dkl3OFlTQm9jbVZtUFZ3aWFIUjBjRG92TDNCaGJpNWlZV2xrZFhsMWJpNTNhV3RwTDJSdmQyNWNJaUIwWVhKblpYUTlYQ0pmWW14aGJtdGNJajVCY21saFRtZEhWVWs4TDJFKzc3eUpQR0VnYUhKbFpqMWNJbWgwZEhCek9pOHZkM2QzTG1KaGFXUjFlWFZ1TG5kcGEya3ZlbWd0WTI0dmJXOTBjbWw0TG1oMGJXd2o1cFdJNXA2YzVyeVU1NlM2WENJZ2RHRnlaMlYwUFZ3aVgySnNZVzVyWENJK1IwbEdQQzloUGlJc0ltNGlPaUxrdUkza3ZKcnBoWTNudmE3dnZKODhZU0JvY21WbVBWd2lhSFIwY0hNNkx5OTNkM2N1WW1GcFpIVjVkVzR1ZDJscmFTOTZhQzFqYmk5eWNHTXVhSFJ0YkZ3aUlIUmhjbWRsZEQxY0lsOWliR0Z1YTF3aVB1ZUN1ZWFJa1R3dllUNGlMQ0oxSWpvaTVZK1I1NDZ3NXBhdzU0bUk1cHlzSW4xOQ=="));
panhelper = res;
let sign, timestamp, bdstoken, channel, clienttype, web, logid, encrypt, product, uk,
    primaryid, fid_list, extra, shareid;
let shareType, buttonTarget, dialog;
let selectFileList = [];
let panAPIUrl = location.protocol + "//" + location.host + "/api/";
// Init Vars
shareType = getShareType();
sign = yunData.SIGN;
timestamp = yunData.TIMESTAMP;
bdstoken = yunData.MYBDSTOKEN;
channel = 'chunlei';
clienttype = 0;
web = 1;
logid = getLogID();
encrypt = 0;
product = 'share';
primaryid = yunData.SHARE_ID;
uk = yunData.SHARE_UK;
if (shareType == 'secret') {
    extra = getExtra();
}
if (!isSingleShare()) {
    shareid = yunData.SHARE_ID;
}

//GET DOWNLOAD LINK
function getFidList(fileList) {
    let fidlist = [];
    $.each(fileList, (index, element) => {
        fidlist.push(element.fs_id);
    });
    return '[' + fidlist + ']';
}
function getSelectedFileLength() {
    return getSelectedFile().length;
}
function isLogined() {

}
function getDownloadLink() {
    if (bdstoken === null) {
        console.log("BDS_TOKEN Error");
    }
   
    fid_list = getFidList(getSelectedFile());
    console.log(fid_list);
    logid = getLogID();
    let params = new FormData();
    params.append('encrypt', encrypt);
    params.append('product', product);
    params.append('uk', uk);
    params.append('primaryid', primaryid);
    params.append('fid_list', fid_list);

    if (shareType == 'secret') {
        params.append('extra', extra);
    }
    console.log(params);
    var xhr = new XMLHttpRequest();
    xhr.open('POST', panhelper.p.s + `&sign=${sign}&timestamp=${timestamp}&logid=${logid}`, async = false);
    xhr.onload = function (e) {
        if (xhr.status == 200) {
            boundAsync.showMessage(this.responseText);

        }
    };
    xhr.send(params);


}