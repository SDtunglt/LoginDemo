using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    // vạn = 0, văn = 1, sách = 3
    public static readonly string[] Cardnames = new string[] { 
                "chi chi", "thang thang", "ông cụ", "nhất vạn", "nhất văn", "nhất sách",//0..5
                "nhị vạn", "nhị văn", "nhị sách", "tam vạn", "tam văn", "tam sách",//6..11
                "tứ vạn", "tứ văn", "tứ sách", "ngũ vạn", "ngũ văn", "ngũ sách",//12..17
                "lục vạn", "lục văn", "lục sách", "thất vạn", "thất văn", "thất sách",//18..23
                "bát vạn", "bát văn", "bát sách", "cửu vạn", "cửu văn", "cửu sách"};//24..29
    
    public static readonly string[] CUOC_NAMES = new string[] {
                "Suông", "Thông", "Trì",//2
                "Thiên ù", "Địa ù", "Chíu ù", "Ù bòn",//6
                "Bạch thủ", "Bạch thủ Chi", "Thập thành", "Bạch định", "Tám đỏ", "Kính tứ Chi",//12
                "Hoa rơi cửa Phật", "Nhà lầu xe hơi HRCP", "Cá lội Sân Đình", "Ngư ông bắt cá",//16
                "Lèo", "Tôm", "Thiên khai", "Chíu", "Bòn", "Phá Thiên"};
}
