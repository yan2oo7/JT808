﻿using JT808.Protocol.MessageBody;
using System;
using System.Collections.Generic;
using Xunit;
using JT808.Protocol.Extensions;
using JT808.Protocol.JT808Formatters;
using JT808.Protocol.Enums;
using Newtonsoft.Json.Linq;
using JT808.Protocol.Attributes;
using Newtonsoft.Json;
using System.Linq;
using System.IO;
using System.Drawing;

namespace JT808.Protocol.Test
{
    public class DemoTest
    {
        [Fact]
        public void Demo1()
        {
            JT808Package jT808Package = new JT808Package();

            jT808Package.Header = new JT808Header
            {
                MsgId = Enums.JT808MsgId.位置信息汇报.ToUInt16Value(),
                MsgNum = 126,
                TerminalPhoneNo = "123456789012"
            };

            JT808_0x0200 jT808_0x0200 = new JT808_0x0200();
            jT808_0x0200.AlarmFlag = 1;
            jT808_0x0200.Altitude = 40;
            jT808_0x0200.GPSTime = DateTime.Parse("2018-10-15 10:10:10");
            jT808_0x0200.Lat = 12222222;
            jT808_0x0200.Lng = 132444444;
            jT808_0x0200.Speed = 60;
            jT808_0x0200.Direction = 0;
            jT808_0x0200.StatusFlag = 2;
            jT808_0x0200.JT808LocationAttachData = new Dictionary<byte, JT808_0x0200_BodyBase>();

            jT808_0x0200.JT808LocationAttachData.Add(JT808_0x0200_BodyBase.AttachId0x01, new JT808_0x0200_0x01
            {
                Mileage = 100
            });

            jT808_0x0200.JT808LocationAttachData.Add(JT808_0x0200_BodyBase.AttachId0x02, new JT808_0x0200_0x02
            {
                Oil = 125
            });

            jT808Package.Bodies = jT808_0x0200;

            byte[] data = JT808Serializer.Serialize(jT808Package);

            var hex = data.ToHexString();
            Assert.Equal("7E02000026123456789012007D02000000010000000200BA7F0E07E4F11C0028003C00001810151010100104000000640202007D01137E", hex);
            // 输出结果Hex：
            // 7E 02 00 00 26 12 34 56 78 90 12 00 7D 02 00 00 00 01 00 00 00 02 00 BA 7F 0E 07 E4 F1 1C 00 28 00 3C 00 00 18 10 15 10 10 10 01 04 00 00 00 64 02 02 00 7D 01 13 7E
        }

        [Fact]
        public void Demo2()
        {
            //1.转成byte数组
            byte[] bytes = "7E 02 00 00 26 12 34 56 78 90 12 00 7D 02 00 00 00 01 00 00 00 02 00 BA 7F 0E 07 E4 F1 1C 00 28 00 3C 00 00 18 10 15 10 10 10 01 04 00 00 00 64 02 02 00 7D 01 13 7E".ToHexBytes();

            //2.将数组反序列化
            var jT808Package = JT808Serializer.Deserialize<JT808Package>(bytes);

            //3.数据包头
            Assert.Equal(Enums.JT808MsgId.位置信息汇报.ToValue(), jT808Package.Header.MsgId);
            Assert.Equal(38, jT808Package.Header.MessageBodyProperty.DataLength);
            Assert.Equal(126, jT808Package.Header.MsgNum);
            Assert.Equal("123456789012", jT808Package.Header.TerminalPhoneNo);
            Assert.False(jT808Package.Header.MessageBodyProperty.IsPackge);
            Assert.Equal(0, jT808Package.Header.MessageBodyProperty.PackageIndex);
            Assert.Equal(0, jT808Package.Header.MessageBodyProperty.PackgeCount);
            Assert.Equal(JT808EncryptMethod.None, jT808Package.Header.MessageBodyProperty.Encrypt);

            //4.数据包体
            JT808_0x0200 jT808_0x0200 = (JT808_0x0200)jT808Package.Bodies;
            Assert.Equal((uint)1, jT808_0x0200.AlarmFlag);
            Assert.Equal((uint)40, jT808_0x0200.Altitude);
            Assert.Equal(DateTime.Parse("2018-10-15 10:10:10"), jT808_0x0200.GPSTime);
            Assert.Equal(12222222, jT808_0x0200.Lat);
            Assert.Equal(132444444, jT808_0x0200.Lng);
            Assert.Equal(60, jT808_0x0200.Speed);
            Assert.Equal(0, jT808_0x0200.Direction);
            Assert.Equal((uint)2, jT808_0x0200.StatusFlag);
            //4.1.附加信息1
            Assert.Equal(100, ((JT808_0x0200_0x01)jT808_0x0200.JT808LocationAttachData[JT808_0x0200_BodyBase.AttachId0x01]).Mileage);
            //4.2.附加信息2
            Assert.Equal(125, ((JT808_0x0200_0x02)jT808_0x0200.JT808LocationAttachData[JT808_0x0200_BodyBase.AttachId0x02]).Oil);
        }

        [Fact]
        public void Demo3()
        {
            JT808Package jT808Package = Enums.JT808MsgId.位置信息汇报.Create("123456789012",
                new JT808_0x0200
                {
                    AlarmFlag = 1,
                    Altitude = 40,
                    GPSTime = DateTime.Parse("2018-10-15 10:10:10"),
                    Lat = 12222222,
                    Lng = 132444444,
                    Speed = 60,
                    Direction = 0,
                    StatusFlag = 2,
                    JT808LocationAttachData = new Dictionary<byte, JT808_0x0200_BodyBase>
                    {
                        { JT808_0x0200_BodyBase.AttachId0x01,new JT808_0x0200_0x01{Mileage = 100}},
                        { JT808_0x0200_BodyBase.AttachId0x02,new JT808_0x0200_0x02{Oil = 125}}
                    }
                });
            jT808Package.Header.MsgNum = 1;
            byte[] data = JT808Serializer.Serialize(jT808Package);
            var hex = data.ToHexString();
            //输出结果Hex：
            //7E 02 00 00 26 12 34 56 78 90 12 00 01 00 00 00 01 00 00 00 02 00 BA 7F 0E 07 E4 F1 1C 00 28 00 3C 00 00 18 10 15 10 10 10 01 04 00 00 00 64 02 02 00 7D 01 6C 7E
            //"7E020000261234567890120001000000010000000200BA7F0E07E4F11C0028003C00001810151010100104000000640202007D016C7E"
            Assert.Equal("7E020000261234567890120001000000010000000200BA7F0E07E4F11C0028003C00001810151010100104000000640202007D016C7E", hex);
        }

        [Fact]
        public void Demo4()
        {
            JT808GlobalConfig.Instance
                // 注册自定义位置附加信息
                .Register_0x0200_Attach(0x06)
                //.SetMsgSNDistributed(//todo 实现IMsgSNDistributed消息流水号)
                // 注册自定义数据上行透传信息
                //.Register_0x0900_Ext<>(//todo 继承自JT808_0x0900_BodyBase类)
                // 注册自定义数据下行透传信息
                //.Register_0x8900_Ext<>(//todo 继承自JT808_0x8900_BodyBase类)
                // 跳过校验码验证
                .SetSkipCRCCode(true);
        }

        /// <summary>
        /// 处理多设备多协议附加信息Id冲突
        /// </summary>
        [Fact]
        public void Demo5()
        {
            JT808GlobalConfig.Instance.Register_0x0200_Attach(0x81);

            JT808Package jT808Package = Enums.JT808MsgId.位置信息汇报.Create("123456789012",
                                                        new JT808_0x0200
                                                        {
                                                            AlarmFlag = 1,
                                                            Altitude = 40,
                                                            GPSTime = DateTime.Parse("2018-12-20 20:10:10"),
                                                            Lat = 12222222,
                                                            Lng = 132444444,
                                                            Speed = 60,
                                                            Direction = 0,
                                                            StatusFlag = 2,
                                                             JT808CustomLocationAttachData=new Dictionary<byte, JT808_0x0200_CustomBodyBase>
                                                             {
                                                                 {0x81,new JT808_0x0200_DT1_0x81 {
                                                                      Age=15,
                                                                      Gender=1,
                                                                      UserName="smallchi"
                                                                 } }
                                                             }
                                                        });

            byte[] data = JT808Serializer.Serialize(jT808Package);
            var jT808PackageResult = JT808Serializer.Deserialize<JT808Package>(data);
            JT808_0x0200 jT808_0X0200 = jT808PackageResult.Bodies as JT808_0x0200;
            var attach = DeviceTypeFactory.Create(cache[jT808PackageResult.Header.TerminalPhoneNo], jT808_0X0200.JT808CustomLocationAttachOriginalData);
            var extJson = attach.ExtData.Data.ToString(Formatting.None);
            var attachinfo81 = (JT808_0x0200_DT1_0x81)attach.JT808CustomLocationAttachData[0x81];
            Assert.Equal((uint)15, attachinfo81.Age);
            Assert.Equal(1, attachinfo81.Gender);
            Assert.Equal("smallchi", attachinfo81.UserName);
        }

        /// <summary>
        /// 处理多媒体数据上传分包处理方式
        /// </summary>
        [Fact]
        public void Demo6()
        {
            JT808GlobalConfig.Instance.SetSkipCRCCode(true);
            //1.首先了解行业的分包策略
            //一般行业分包是按256的整数倍，太多不行，太少也不行，必须刚刚好
            //例:这边以256的3倍进行处理   
            var quotient = 6935 / (256 * 3);
            var remainder = 6935 % (256 * 3);
            if (remainder != 0)
            {
                quotient = quotient + 1;
            }
            Assert.Equal(23, remainder);
            Assert.Equal(10, quotient);
            //得到有10包的数据

            //2.根据以上信息，来解分包数据
            //第一包 有具体信息+多媒体  
            //第二包 多媒体数据
            //...
            //最N包  多媒体数据
            byte[] bytes1 = "7e080123240138123456782031000a00010000271d0000010100000000000c200301d2400a063296a501e100000000190104092952ffd8ffc4001f0000010501010101010100000000000000000102030405060708090a0bffc400b5100002010303020403050504040000017d0101020300041105122131410613516107227114328191a1082342b1c11552d1f02433627282090a161718191a25262728292a3435363738393a434445464748494a535455565758595a636465666768696a737475767778797a838485868788898a92939495969798999aa2a3a4a5a6a7a8a9aab2b3b4b5b6b7b8b9bac2c3c4c5c6c7c8c9cad2d3d4d5d6d7d8d9dae1e2e3e4e5e6e7e8e9eaf1f2f3f4f5f6f7f8f9faffc4001f0100030101010101010101010000000000000102030405060708090a0bffc400b51100020102040403040705040400010277000102031104052131061241510761711322328108144291a1b1c109233352f0156272d10a162434e125f11718191a262728292a35363738393a434445464748494a535455565758595a636465666768696a737475767778797a82838485868788898a92939495969798999aa2a3a4a5a6a7a8a9aab2b3b4b5b6b7b8b9bac2c3c4c5c6c7c8c9cad2d3d4d5d6d7d8d9dae2e3e4e5e6e7e8e9eaf2f3f4f5f6f7f8f9faffdb004300080606070605080707070909080a0c140d0c0b0b0c1912130f141d1a1f1e1d1a1c1c20242e2720222c231c1c2837292c30313434341f27393d38323c2e333432ffdb0043010909090c0b0c180d0d1832211c213232323232323232323232323232323232323232323232323232323232323232323232323232323232323232323232323232fffe000b4750456e636f646572ffdb0043000d090a0b0a080d0b0a0b0e0e0d0f13201513121213271c1e17202e2931302e292d2c333a4a3e333646372c2d405741464c4e525352323e5a615a50604a51524fffdb0043010e0e0e131113261515264f352d354f4f4f4f4f4f4f4f4f4f4f4f4f4f4f4f4f4f4f4f4f4f4f4f4f4f4f4f4f4f4f4f4f4f4f4f4f4f4f4f4f4f4f4f4f4f4f4f4f4fffc000110800f0014003012100021101031101ffc4001f000001050101010101010000000000000000010203041c7e".ToHexBytes();
            byte[] bytes2 = "7e080123000138123456782032000a000205060708090a0bffc400b5100002010303020403050504040000017d0101020300041105122131410613516107227114328191a1082342b1c11552d1f02433627282090a161718191a25262728292a3435363738393a434445464748494a535455565758595a636465666768696a737475767778797a838485868788898a92939495969798999aa2a3a4a5a6a7a8a9aab2b3b4b5b6b7b8b9bac2c3c4c5c6c7c8c9cad2d3d4d5d6d7d8d9dae1e2e3e4e5e6e7e8e9eaf1f2f3f4f5f6f7f8f9faffc4001f0100030101010101010101010000000000000102030405060708090a0bffc400b51100020102040403040705040400010277000102031104052131061241510761711322328108144291a1b1c109233352f0156272d10a162434e125f11718191a262728292a35363738393a434445464748494a535455565758595a636465666768696a737475767778797a82838485868788898a92939495969798999aa2a3a4a5a6a7a8a9aab2b3b4b5b6b7b8b9bac2c3c4c5c6c7c8c9cad2d3d4d5d6d7d8d9dae2e3e4e5e6e7e8e9eaf2f3f4f5f6f7f8f9faffdd00040000ffda000c03010002110311003f006c6a2a755ce299a5c942e0f35281c5004aa72314a54e38a07b8841ef4840a0673de21b4ff498ee402038dade991fe7f4acc110f4a0cd8ef2f1405cd01d45f2e9360a062edc5745616a6dad511861cfccff0053499512e056cf1460e3348a0ed4b8e338fc2819cb5edbfd9ee648b18556f97fdd3d3f4aafb4d332ea433a6573e9550d3131d18c9c558031c0a4083a503039a60c42c2984e4f4a06260d370690098ef4751400c615132d021868a621431a33480ef235e05595403eb54cbb0b8e7069dc0e3a9a41b12a024f4a9d40f4a18c5651e951c88179268194ee614b989a2719461ffea35cfdcda4b6b2ed71c1e55874345c96ba91819a704c50217613db349b39031c9e945c66a69ba794713cebf30fb8be9ee6b540c1e948a48760e3a526d2dc77a0a144471d297cb623a71484646bb685234b81d01d8e7d018f43f9ff003ac16386c552225b8300c2a84c8c8c4ed247b502616cc0517e".ToHexBytes();
            byte[] bytes3 = "7e080123000138123456782033000a0003b104558fe2c0a0561a4a2fde603ea69a67887f10fca80b8c33a960aa0b13d001d6a4f2e73d2d2e3f189a906e42653fdd23eb40dec3229dc7b91bc855b19e69c8dbc7d29081bad308a2e0308a6e281094b4c0efd41152a83815459623518c9a4000391f852192ad4c89923340131031c5579327822a4645e5f534bf6649c794f109037f0e33f8fb530226f0a1621a39bca1dc30dd4f8fc2d1a91be791cfb1007f2a9bb15916a3f0f59a0cb4793ebb98ff005a6369f6d6f70c6285548c60e327a5090f4e82f94b8e050235eb8cd3192000f6a76063a0a62b8840e83ad3447d02f487723b8b549edde09092b229527fad70d3c4e8ed1b7d01e462a7ea0e0d344b05538a8a7520676d0495611fbd1907a54ea3e623de80335d8b4849eb46334124d6937d019aee19b3808e0b7d013a1fd09aebc001871d0e2933481c83fdf6efc9fe75620c6de693d89452bb189734d82508ff0037434c4f72c821864546d4c08c9a4ed4084cd25203d284240e68db8e2acb628cf4a72825b9a2c05855e47bd4c1690c5ee39fca8f2c1393d2900151c8c0ad2b1882401f1f31efed43d81ec583498f6a424348acd9c66773ef8a10c6600c534af3c530142f7c52ede2818a073cd21a4034d739e21b411dca5c8185986d6f671fe23f9500f63194e1b069d20057a5324a9b3f7a1881c53e4555607d0168119772bb6e5fd09c8fc69b412275eb5d468f78b736b1c6cdfbd880571dc81d1bfcf7a4ca8bb330580dc40f53fcea488606293d86ad733e6ddb8ef3920d474c82cdbb020a9fc2a465c8a0688186293b5021292803d594074e7f0a89a160724552346089baa558c861c67f0a6c0777e94aa0e79a901ebef4fe31400a3a678ad4b7188231fecd0c4c7d01252101acc941f31ffde3fce84086e3bd2ed0062994252f14009c629b814804e38f5aaf7d016ab79672db9382c32a4f661d0d0338a91086391b4824303d88ea29c872281113a60f34c7e573e94c452d413947f6c5550322826c1814a32ac1d58ab0e841c11f8d0200cc3bd289a453f2b63f006931a644e4b1249e6a3a043a27dae0d5cc823340d0d2aa7ad3590638a632e77e".ToHexBytes();
            byte[] bytes4 = "7e080123000138123456782035000a00041208ed499f6a449ead03f1524846474abb1a0232820e29dc1e7ad20184e7a52a939a180f3c1a073dc502141c02066b5e2ff5298fee8a4c18b4b484277acb392324fbd34026de393d280a31c0a0a178f4a3ad001f8521a402714868039bf1159ecb85b940764dc37b3fff005c7f235891b6d6c1a043a5191915120c8c1a68457ba42d6ad9c165e7fc6b3874a04c3bd5ab3837923ece66661951e66deffafe74304421148ce08cfbd396dc367d026c52b82d4ad2aec62b9ce38a84d0480abb6bf347f8d0344a5451b01a650d31835198850267a4c6c78c54c32464d51403f3a777ce39fa5001c8e9d68f28c992cdc7a6290c9428005017b81400e00107e95ad19fdd27fba293131b24b1c632ed8cfe755daf3fb8871eadfe148446d7331fe20bf4150e6a876179c507ebcd2013bd2f7d02b40c303bd2607ad201300f4a69c77a01115ddba5d5bbc127dd71d7fba7b1fceb89ba824b79de39542ba1c30f5f71ed4d0d8d07726291461a8246b2fcecad92a47d029d0d63152ae55ba8e0d02618abb6ccb04f6f234ea132a5846fc8ee411498968439c28fa5393279ed40d22adcae253ef55cd0896255bb161b8a9eb8c8a016e5a75a68e0505aee148453623d063391565785e6ada180fa714ea401d3a5381cf7a00909e3af3429f97d01a900efa035a687f769fee8fe552c199ef966663f7893d699c81540213f4a33400bc91ed41ebd28181e7bd02900beb49c039a004e868c7ad218d3d39ac9d774ff00b4422e625ccb18c3003efaff0088ff001f6a019cc018e54e41a53c72699224bfc2c2b2af976cf9ecdcd026419ab576e860b4db22bb2c655b6f6e8403ef834988801cf34fdc7a0a0115e62c4fcc7245407ad026253a362ac18751c8a048d546124418742298460f34cd3a077a4228b08f404ed8a9d73deb41920507a52e30290213b50bd793480939a7a807f1a403fa0eb5a31f31a7a6d1fca930651907ccdfef1fe74dc647340119ebc11477a602f34bc628003d29052016909a0043cfad21e281a003e5e691b83c1a451ce6b3a68819aea15fdcb1f9947f013dfe9fcab16746d9f2119a09636305f17e".ToHexBytes();
            byte[] bytes5 = "7e080123000138123456782036000a0005e0d87395e39aa77d02bbe049076feb4c9281276123ae2b5756b69223120c1853708f1db2727f1e83f0a4dd848cec1cfd29caac4f14011dcc4ea4b31041f4aaadd681356128140917ac64f98c4ddf95feb53bafcd4ee5a0e945219e8082a55ad009948c0a0f5e690098e694601e68043875229c0f6348078208c55813b9558e24e40029010bee048239cf34c24fe1400879e86933c530173c71450028e68e7b5200cf1cd1d28189cf7a6f7a0033e941a431ac0104100823041ef5ce6a7a5b5bee96005adfa95ef1ff0088a00c644f2dfe5fba6a3963dc92463b8247f9fad3259931a6f9163e9bc85fcf8aeb6f915ecae5994102372323b80706a5847a9cb370dc5390f4e69b12dc5bbe62ace6a5109ee349a01a64122b302194e083915a88e254571d08fca8452131cd25319e82bd29ea735a0c91781c714b4807720d1d4d20141ff38a7fe14980e19c702b4605558548ee326a58156e062673ea47f21509071c53431a69b4c42f238a5140071d28a402fd68ebcd0310b718a414001e3ad44cf8e00cd21d803315191cd009c66819caf8996dec6680c3195926dc59411b703b8f7c9e9d2b145d6eb80a5782700e7f2a086ecc86e54db5d4770abb9438703d483922b7deee0bad3259607d01ca40057bae481823f1a182dce6dc0dec3d09a68386cd0224b878cc240910923b106b3de9209319494c81c2ae58cbb4b467a1e47d017bd0868b8453282cf425c60669c0815a88703e94e1f5a4317a914b9a005079a729f7a404d1f4e6b42123c95e73c5430655bac79bf85454c6842335191838a10064668ce7b531052e6818123346714804c8cf14848cd16018fcf5a3e5ce33cd2283349cd202b5e5a5ade2aa5ddba4a14e5438e95c8788eca1b3d563682210c32c61f0a30b9048381db803f3a052d88595590c728f620f5154e4b22afba07cfe383f9d321a20659d3ef231f7c66a32f92322810e7784a711b06c7d021fceaab74a1098ca4a04396a48d5f70d80ee078a011aad8e7151e683447a103c5387bd6a214714e07b5218eed49cd210b5228cd0c68915702afdb7302fe3fcea58d8c9e2df8651cd46b17e".ToHexBytes();
            byte[] bytes6 = "7e080123000138123456782037000a00062d98fde200fce95c02689121cae4b64724d553d734d0098a2988075e29739a061c75a43d6900a00f7cd21a010d23351f92a4e49a4324031c0a42300fafa503216697236c6847bb91fd2b03c636cd2e9b0cdb483149b49073856183fa85a2e26b42e2edbdd3ade5b8b433bcb023332ed0725413825811595aa69d0c16ed730a5cc2c1954233232f240f527f5a416b984d75872acac307069a6ef9fbb914c812496129cc0c0b742c8067f1aa0d4098ca4a091d19c383ef5a8846060505445249ed4d39c5051e8601ed4a2b5174179a72d201e791d680290583daa44f7340d130e956ed8feeb1e86a18c968a40417471171d73548903ad5201327a51d45301474a43d78a041d283f5a062134bee6800ed4dcd21a133c51d4521dc43ef54758b6375a45dc001666889503a961cafea0520650f0ccab2e816e158b142e873dbe6240fc88a678924f2ec2200f0f3a839f4dac7f9814093d0e36521a6761ce5891f9d307d01ea642dcd28119e103cc700f1818ac871c9a888e6b44454559980abf6d306f948e719a0a44ae48a8c927bd0ca3d21474a76dad405ed4a05210a4d00fbd0028fcaa44cd26324c9c60d5ab5276107d6a58c9ba5213807daa40a72c85db3dbb66a02393d7156803b51da80141a0d00379cd19e6980734a6a404cfe2293ad030cf3d69290087ad267041f439a0673de1b516a752d3c2b0fb3dc92a4f752303f44cfe351f8ad87d8201d499f3f9237f88a5d457f74e489e4d20eb4cccd4b63fba0738ac89861d87bd4c772e7b101a43546402acdb1fde7e140e25966cd309a451e96b8c53b391c56cc032334b9ed4ba007229473400e5f5ef5203498c783ed566d8f2c3f1a96327c8f4aad7126ef917a0eb490105348f6a60373da81f4aa00e334669083a77d02b49939a00075a318340c3f4a0e290c4ef487af1486235337608c73cd0c673da0e9c63b896f9aefce626481814e490e0649cfa2838c77aafe2e3f2d90c9c7ef4ffe81fe34913f64e58f5a55049e29999ab021f2c2e39ac8b9c79cf8e066a22f52e7b15cd2559900a9adff00d60a06b72e374a61a459e97818a33c735b517e".ToHexBytes();
            byte[] bytes7 = "7e080123000138123456782039000a0007083753860d0201eb474249a431c0e3be69e0feb40c78e9cd4f6ee164e4f0c3152c09e57f2d38fbc7a553e49e7a9a48628cd205c0ed400df6a4ebc835420a3a74a0043463b5201067d453bad0310d2521852646290c6139a6e307e94018fa080a9a8afa6a330ffd06b27c5b213776f1e785889c7d015bff00b1fd2844bd8e74d2ae148dd43211785f4318eacc7fd95ff1ace9dc492b3818c92706a522e72bab101a61aa320152c270e28045eea29bde91a1e900e40ce29df4ad84181fe34bdb148017a7ad29191c1e28004523bfeb52af4a0687678a5cf03fc6900fdc4f2c72693d719a40814f34ee3191487718c78f534cfa55210527d2900528a6301d68ed4803eb49f5a43427e34879a430f6a6b71de90185a54d1c17fa9d9cce893bde3cc8acc01757c118f5e954f58b34bef10c56f2dca5b462d4349230e803bf41dc9240fc68b93ba33aef4db479c47a79b928bd65b8651bbaf450338fa9cfb53e3b3d3e1426e2de795c63ef30c1fa053fce84db1596e4332d938262b344278c3123f406b1e7016420631ed4214add080d34d32029e870c0d008be0fca28a469d0f46538a767dab6b08307ae452e79a02c3c0f6a5e280157834ef6a43428a5079e2900e5a53de90c4ce0d283400d61f5a667d29a00e293340833cd3a90013471c7340c4a3ad218d079a53eff00a5201a698719e681952fac6d2f902dd4092606031e187d0108e4571da9eed3b549618a5790441510ca7710a541c7b637629589969b14cea175962b22ae7fba8bf9722a169e763969a4fc188a6436205790e0b649fef1a8e58cc6d8241fa52b85b42134ca64853875a00bf09cc629c691a743d1507a53803bab7247f51487afad20147b529a060bc53b3f4a40283c734e1dbd6818e1c1a71a402639a00f5a0071195a85bae69218ccf3d697b5310718a39ec2801d9a09e290c4cf3d68ef498c4cf3f4a42690210f4a6e05031ae462b83f104825d5ee88c7126dff00be542ffecb40a5b1979a51419a244201a8e76dcf483a15cf5a6d3244a506802edb1fddd48c78a562d6c7a42e78152003ad6c21718a61ebc503140e79a0f5c57e".ToHexBytes();
            byte[] bytes8 = "7e08012300013812345678203a000a0008e050003d69c39148078a75030073cd2e49a402819a4ce38a100ecf1cd31c11490c8f1ef4d2714c419e697340200d9e28c8cd0302e0537713d2900b839a711523108f6a61e940d0c6e481d2bcef527126a172ebd1a7908ffbecd3265b1529b9a4412c43229938c3500f62b93494122502802dda9e08a9cfa522d6c7a4af4a7027b56cc07751cd37239a003349df02801ca29e01cd201e07ad35b9c8eded4863870296800fc6900e7d01e818ece3a8e29924c8382d4ac0425d4f3918fcf346e07bfe54c41de940e280414c7ce06290d0880f7a9028a40380e48a5268631b8ebe9519e3a5201bbb69dc7a0e4d798e7f7683fd914132129941058807cb51dce7752ea37b158d253244a050059b53f362ad1eb525ad8f4843c52b673c56e02eee314dcf6a40380cf6a728e78a02c380a7724d0324c7151b019ef48031c7b8a51db8c9a00701bbd2823029019f7b3b2f0a08acc6f3a46ea49f6a4d945eb585f6fef2ae2a63914d09ee3b1487a5315c074a303bd200e94a39a063b9141e9d290d0c272714d3d7a74a4329ea6e63d32f1c1c15b79083efb4e2bcea4c6ec0e8381411313b532820b76e3e5a86ec7cf496e53d8aa69299025140135b9c3d5c26917167a5a8c2d2119e6b604369ebc9a043a9573e940c701eb4ec60648a43149fca8efcd201a41fafd2973d38a00907e82931da9011490abff0d462dd57955c51618e031d051f8d3105250c04c668c7a50160fc29d8e3b5218a38a3ad218d34da00cbf11b32e857454e090a99f666507f426b8173f3923d6826436900e69106a4501f211d57aa838aa179feb2a22f53492b22a9eb4dab3210d1401243f7c55ea0a47a59e94bdb9ad46263238a700062801ddf9a075a063874e3a0a5038a4027d290668121738a51d7148a2651c507bfa5480da461ef4c061fc2986a84250460520003bf5cfa526da005007526979a06264fad2e38a4034e477a692690cc3f16c853485453feb27453f400b7f3515c4b0e68225b801c53e38c33804f7a4248d9b71b620a3a0e0565ea8a05c123bd4477349af74cf34dab310c5262801f1fdf15a1fc2291713d29e17e".ToHexBytes();
            byte[] bytes9 = "7e08012300013812345678203b000a000979e3a53f19c56e00571401c83e94807751477a062f7a334806939a51400ee714e51ce6900fcf6a5c8c62a4634900e05464f38a6806f4149926a8570fc281f4a00300f5147514804cf38ef4a46690d09c62973e94980c6e05373c734586735e309084b3887d01d66773f50001ff00a11ae5475a4449ea3a917a8a03a9b96e331afd05656abff1f1f854477349fc2671a6d5988525021cbf785690c14071499713d1d6a5e715b80b8a41c5031e3d7a51838a9005ce6822801b8c7ffaa9caa3a8a00514f0063ae6a406938e9c500d3189c93d29ac0d0210d3698801e334eea3fa50301411405841cd21a430f6a09348686923bd37a0c0a40723e2d918ea11467d02ea4008fab31cff00e822b0145043dc5229557914811b36ff00eac7d2b2b533998d4477349ec67914d356602514000eb5a28dfbb1499513d2e300549c56c317391d29b8a009171dea4c6471c5263136e39a69eb4806d2e71da980139e3934a091ee29580060d03b71de900a00fad35a980d6a691d85001e94a05300ed49ef4009c75c51d690c404678a2a410c6a6ee3eb40d1c4788df76b574339d9b147e083fa9359a9410c76053d00dc2902352318502b2750e6723d2a62692d8a2d4d35460211498a003bd5f888f2c50ca47a62364022a4072335b0c55c9a5c67b5218a3838e69eadf4a403b3c534f348031c51b7d2800dbcf4a52bdfad201b4ee9cd00216fad464f3c9cfd280140e39cd3714c050b498e7ad0027b671450c2e21e4534d0019f6a439a43434f4e69a4f6148a380d4dfccd46e9f76eccefce7a80c40fd00aacb4198a4e29eae030c52046ba8205635f9ff486a989a4f6291eb486a8c46d250201d6ae43f7052291e8f6d28c0c9e2ae2e36e715b8c701de9c0fd6a405e0f14a3e940c3bd2e38a4029e3147005200f4a427b50027f5a4ddda9821a7149804d031c41e9498f4033ed400e031d69ac31d29086f5a4fd6a806fe94948000a53d3148a18c31e86a3670877b7217938f41480f36e91203fdd1403c506605b1525b61a6553d09a4f61adcdf8903b2a138cfa75c77ac1d54a7db64f2d76a6e3b46738fc6b38b6b7e".ToHexBytes();
            byte[] bytes10= "7e08012017013812345678203c000a000ad4dea2f72e50269b5a1ca251400a2ad427e5a4ca47ffd9d17e".ToHexBytes();

            JT808Package jT808_0X0801_1 = JT808Serializer.Deserialize<JT808Package>(bytes1);
            JT808Package jT808_0X0801_2 = JT808Serializer.Deserialize<JT808Package>(bytes2);
            JT808Package jT808_0X0801_3 = JT808Serializer.Deserialize<JT808Package>(bytes3);
            JT808Package jT808_0X0801_4 = JT808Serializer.Deserialize<JT808Package>(bytes4);
            JT808Package jT808_0X0801_5 = JT808Serializer.Deserialize<JT808Package>(bytes5);
            JT808Package jT808_0X0801_6 = JT808Serializer.Deserialize<JT808Package>(bytes6);
            JT808Package jT808_0X0801_7 = JT808Serializer.Deserialize<JT808Package>(bytes7);
            JT808Package jT808_0X0801_8 = JT808Serializer.Deserialize<JT808Package>(bytes8);
            JT808Package jT808_0X0801_9 = JT808Serializer.Deserialize<JT808Package>(bytes9);
            JT808Package jT808_0X0801_10 = JT808Serializer.Deserialize<JT808Package>(bytes10);

            var jT808_0X0801_body_1 = jT808_0X0801_1.Bodies as JT808_0x0801;
            var jT808_0X0801_body_2 = jT808_0X0801_2.Bodies as JT808SplitPackageBodies;
            var jT808_0X0801_body_3 = jT808_0X0801_3.Bodies as JT808SplitPackageBodies;
            var jT808_0X0801_body_4 = jT808_0X0801_4.Bodies as JT808SplitPackageBodies;
            var jT808_0X0801_body_5 = jT808_0X0801_5.Bodies as JT808SplitPackageBodies;
            var jT808_0X0801_body_6 = jT808_0X0801_6.Bodies as JT808SplitPackageBodies;
            var jT808_0X0801_body_7 = jT808_0X0801_7.Bodies as JT808SplitPackageBodies;
            var jT808_0X0801_body_8 = jT808_0X0801_8.Bodies as JT808SplitPackageBodies;
            var jT808_0X0801_body_9 = jT808_0X0801_9.Bodies as JT808SplitPackageBodies;
            var jT808_0X0801_body_10 = jT808_0X0801_10.Bodies as JT808SplitPackageBodies;
            var imageBytes = jT808_0X0801_body_1.MultimediaDataPackage
                                            .Concat(jT808_0X0801_body_2.Data)
                                            .Concat(jT808_0X0801_body_3.Data)
                                            .Concat(jT808_0X0801_body_4.Data)
                                            .Concat(jT808_0X0801_body_5.Data)
                                            .Concat(jT808_0X0801_body_6.Data)
                                            .Concat(jT808_0X0801_body_7.Data)
                                            .Concat(jT808_0X0801_body_8.Data)
                                            .Concat(jT808_0X0801_body_9.Data)
                                            .Concat(jT808_0X0801_body_10.Data).ToArray();
            //using (MemoryStream ms = new MemoryStream(imageBytes))
            //{
            //    Image image = Image.FromStream(ms);
            //    image.Save("test.jpeg");
            //}
        }

        private Dictionary<string, DeviceType> cache = new Dictionary<string, DeviceType>
        {
            { "123456789012",DeviceType.DT1 },
            { "123456789013",DeviceType.DT2 }
        };

        public interface IExtData
        {
            JObject Data { get; set; }
        }

        public interface IExtDataProcessor
        {
            void Processor(IExtData extData);
        }

        public class JT808_0x0200_DT1_0x81_ExtDataProcessor : IExtDataProcessor
        {
            private JT808_0x0200_DT1_0x81 jT808_0X0200_DT1_0X81;
            public JT808_0x0200_DT1_0x81_ExtDataProcessor(JT808_0x0200_DT1_0x81 jT808_0X0200_DT1_0X81)
            {
                this.jT808_0X0200_DT1_0X81 = jT808_0X0200_DT1_0X81;
            }
            public void Processor(IExtData extData)
            {
                extData.Data.Add(nameof(JT808_0x0200_DT1_0x81.Age), jT808_0X0200_DT1_0X81.Age);
                extData.Data.Add(nameof(JT808_0x0200_DT1_0x81.UserName), jT808_0X0200_DT1_0X81.UserName);
                extData.Data.Add(nameof(JT808_0x0200_DT1_0x81.Gender), jT808_0X0200_DT1_0X81.Gender);
            }
        }

        public class JT808_0x0200_DT1_0x82_ExtDataProcessor : IExtDataProcessor
        {
            private JT808_0x0200_DT1_0x82 jT808_0X0200_DT1_0X82;
            public JT808_0x0200_DT1_0x82_ExtDataProcessor(JT808_0x0200_DT1_0x82 jT808_0X0200_DT1_0X82)
            {
                this.jT808_0X0200_DT1_0X82 = jT808_0X0200_DT1_0X82;
            }
            public void Processor(IExtData extData)
            {
                extData.Data.Add(nameof(JT808_0x0200_DT1_0x82.Gender1), jT808_0X0200_DT1_0X82.Gender1);
            }
        }

        public class JT808_0x0200_DT2_0x81_ExtDataProcessor : IExtDataProcessor
        {
            private JT808_0x0200_DT2_0x81 jT808_0X0200_DT2_0X81;
            public JT808_0x0200_DT2_0x81_ExtDataProcessor(JT808_0x0200_DT2_0x81 jT808_0X0200_DT2_0X81)
            {
                this.jT808_0X0200_DT2_0X81 = jT808_0X0200_DT2_0X81;
            }
            public void Processor(IExtData extData)
            {
                extData.Data.Add(nameof(JT808_0x0200_DT2_0x81.Age), jT808_0X0200_DT2_0X81.Age);
                extData.Data.Add(nameof(JT808_0x0200_DT2_0x81.Gender), jT808_0X0200_DT2_0X81.Gender);
            }
        }

        public class DeviceTypeFactory
        {
            public static DeviceTypeBase Create(DeviceType deviceType, Dictionary<byte, byte[]> jT808CustomLocationAttachOriginalData)
            {
                switch (deviceType)
                {
                    case DeviceType.DT1:
                        return new DeviceType1(jT808CustomLocationAttachOriginalData);
                    case DeviceType.DT2:
                        return new DeviceType2(jT808CustomLocationAttachOriginalData);
                    default:
                        return default;
                }
            }
        }

        public enum DeviceType
        {
            DT1=1,
            DT2=2
        }

        public abstract class DeviceTypeBase
        {
            protected class DefaultExtDataImpl : IExtData
            {
                public JObject Data { get; set; } = new JObject();
            }
            public virtual IExtData ExtData { get; protected set; } = new DefaultExtDataImpl();
            public abstract  Dictionary<byte, JT808_0x0200_CustomBodyBase> JT808CustomLocationAttachData { get; protected set; }
            protected DeviceTypeBase(Dictionary<byte, byte[]> jT808CustomLocationAttachOriginalData)
            {
                Execute(jT808CustomLocationAttachOriginalData);
            }
            protected abstract void Execute(Dictionary<byte, byte[]> jT808CustomLocationAttachOriginalData);
        }

        public class DeviceType1 : DeviceTypeBase
        {
            private const byte dt1_0x81 = 0x81;
            private const byte dt1_0x82 = 0x82;
            public DeviceType1(Dictionary<byte, byte[]> jT808CustomLocationAttachOriginalData) : base(jT808CustomLocationAttachOriginalData)
            {
            }
            public override Dictionary<byte, JT808_0x0200_CustomBodyBase> JT808CustomLocationAttachData { get; protected set; }

            protected override void Execute(Dictionary<byte, byte[]> jT808CustomLocationAttachOriginalData)
            {
                JT808CustomLocationAttachData = new Dictionary<byte, JT808_0x0200_CustomBodyBase>();
                foreach(var item in jT808CustomLocationAttachOriginalData)
                {
                    try
                    {
                        switch (item.Key)
                        {
                            case dt1_0x81:
                                var info81 = JT808Serializer.Deserialize<JT808_0x0200_DT1_0x81>(item.Value);
                                if (info81 != null)
                                {
                                    IExtDataProcessor extDataProcessor = new JT808_0x0200_DT1_0x81_ExtDataProcessor(info81);
                                    extDataProcessor.Processor(ExtData);
                                    JT808CustomLocationAttachData.Add(dt1_0x81, info81);
                                }
                                break;
                            case dt1_0x82:
                                var info82 = JT808Serializer.Deserialize<JT808_0x0200_DT1_0x82>(item.Value);
                                if (info82 != null)
                                {
                                    IExtDataProcessor extDataProcessor = new JT808_0x0200_DT1_0x82_ExtDataProcessor(info82);
                                    extDataProcessor.Processor(ExtData);
                                    JT808CustomLocationAttachData.Add(dt1_0x82, info82);
                                }
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        
                    }
                }
            }
        }

        public class DeviceType2 : DeviceTypeBase
        {
            public DeviceType2(Dictionary<byte, byte[]> jT808CustomLocationAttachOriginalData) : base(jT808CustomLocationAttachOriginalData)
            {
            }
            public override Dictionary<byte, JT808_0x0200_CustomBodyBase> JT808CustomLocationAttachData { get; protected set; }

            private const byte dt2_0x81 = 0x81;

            protected override void Execute(Dictionary<byte, byte[]> jT808CustomLocationAttachOriginalData)
            {
                JT808CustomLocationAttachData = new Dictionary<byte, JT808_0x0200_CustomBodyBase>();
                foreach (var item in jT808CustomLocationAttachOriginalData)
                {
                    try
                    {
                        switch (item.Key)
                        {
                            case dt2_0x81:
                                var info81 = JT808Serializer.Deserialize<JT808_0x0200_DT2_0x81>(item.Value);
                                if (info81 != null)
                                {
                                    IExtDataProcessor extDataProcessor = new JT808_0x0200_DT2_0x81_ExtDataProcessor(info81);
                                    extDataProcessor.Processor(ExtData);
                                    JT808CustomLocationAttachData.Add(dt2_0x81, info81);
                                }
                                break;
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }

        /// <summary>
        /// 设备类型1-对应消息协议0x81
        /// </summary>
        [JT808Formatter(typeof(JT808_0x0200_DT1_0x81Formatter))]
        public class JT808_0x0200_DT1_0x81 : JT808_0x0200_CustomBodyBase
        {
            public override byte AttachInfoId { get; set; } = 0x81;
            public override byte AttachInfoLength { get; set; } = 13;
            public uint Age { get; set; }
            public byte Gender { get; set; }
            public string UserName { get; set; }
        }
        /// <summary>
        /// 设备类型1-对应消息协议0x82
        /// </summary>
        public class JT808_0x0200_DT1_0x82 : JT808_0x0200_CustomBodyBase
        {
            public override byte AttachInfoId { get; set; } = 0x82;
            public override byte AttachInfoLength { get; set; } = 1;
            public byte Gender1 { get; set; }
        }
        /// <summary>
        /// 设备类型2-对应消息协议0x81
        /// </summary>
        public class JT808_0x0200_DT2_0x81 : JT808_0x0200_CustomBodyBase
        {
            public override byte AttachInfoId { get; set; } = 0x81;
            public override byte AttachInfoLength { get; set; } = 7;
            public uint Age { get; set; }
            public byte Gender { get; set; }
            public ushort MsgNum { get; set; }
        }
        /// <summary>
        /// 设备类型1-对应消息协议序列化器 0x81
        /// </summary>
        public class JT808_0x0200_DT1_0x81Formatter : IJT808Formatter<JT808_0x0200_DT1_0x81>
        {
            public JT808_0x0200_DT1_0x81 Deserialize(ReadOnlySpan<byte> bytes, out int readSize)
            {
                int offset = 0;
                JT808_0x0200_DT1_0x81 jT808_0X0200_DT1_0X81 = new JT808_0x0200_DT1_0x81();
                jT808_0X0200_DT1_0X81.AttachInfoId = JT808BinaryExtensions.ReadByteLittle(bytes, ref offset);
                jT808_0X0200_DT1_0X81.AttachInfoLength = JT808BinaryExtensions.ReadByteLittle(bytes, ref offset);
                jT808_0X0200_DT1_0X81.Age = JT808BinaryExtensions.ReadUInt32Little(bytes, ref offset);
                jT808_0X0200_DT1_0X81.Gender = JT808BinaryExtensions.ReadByteLittle(bytes, ref offset);
                jT808_0X0200_DT1_0X81.UserName = JT808BinaryExtensions.ReadStringLittle(bytes, ref offset);
                readSize = offset;
                return jT808_0X0200_DT1_0X81;
            }

            public int Serialize(ref byte[] bytes, int offset, JT808_0x0200_DT1_0x81 value)
            {
                offset += JT808BinaryExtensions.WriteByteLittle(bytes, offset, value.AttachInfoId);
                offset += JT808BinaryExtensions.WriteByteLittle(bytes, offset, value.AttachInfoLength);
                offset += JT808BinaryExtensions.WriteUInt32Little(bytes, offset, value.Age);
                offset += JT808BinaryExtensions.WriteByteLittle(bytes, offset, value.Gender);
                offset += JT808BinaryExtensions.WriteStringLittle(bytes, offset, value.UserName);
                return offset;
            }
        }
        /// <summary>
        /// 设备类型1-对应消息协议序列化器 0x82
        /// </summary>
        public class JT808_0x0200_DT1_0x82Formatter : IJT808Formatter<JT808_0x0200_DT1_0x82>
        {
            public JT808_0x0200_DT1_0x82 Deserialize(ReadOnlySpan<byte> bytes, out int readSize)
            {
                int offset = 0;
                JT808_0x0200_DT1_0x82 jT808_0X0200_DT1_0X82 = new JT808_0x0200_DT1_0x82();
                jT808_0X0200_DT1_0X82.AttachInfoId = JT808BinaryExtensions.ReadByteLittle(bytes, ref offset);
                jT808_0X0200_DT1_0X82.AttachInfoLength = JT808BinaryExtensions.ReadByteLittle(bytes, ref offset);
                jT808_0X0200_DT1_0X82.Gender1 = JT808BinaryExtensions.ReadByteLittle(bytes, ref offset);
                readSize = offset;
                return jT808_0X0200_DT1_0X82;
            }

            public int Serialize(ref byte[] bytes, int offset, JT808_0x0200_DT1_0x82 value)
            {
                offset += JT808BinaryExtensions.WriteByteLittle(bytes, offset, value.AttachInfoId);
                offset += JT808BinaryExtensions.WriteByteLittle(bytes, offset, value.AttachInfoLength);
                offset += JT808BinaryExtensions.WriteByteLittle(bytes, offset, value.Gender1);
                return offset;
            }
        }
        /// <summary>
        /// 设备类型2-对应消息协议序列化器 0x81
        /// </summary>
        public class JT808_0x0200_DT2_0x81Formatter : IJT808Formatter<JT808_0x0200_DT2_0x81>
        {
            public JT808_0x0200_DT2_0x81 Deserialize(ReadOnlySpan<byte> bytes, out int readSize)
            {
                int offset = 0;
                JT808_0x0200_DT2_0x81 jT808_0X0200_DT2_0X81 = new JT808_0x0200_DT2_0x81();
                jT808_0X0200_DT2_0X81.AttachInfoId = JT808BinaryExtensions.ReadByteLittle(bytes, ref offset);
                jT808_0X0200_DT2_0X81.AttachInfoLength = JT808BinaryExtensions.ReadByteLittle(bytes, ref offset);
                jT808_0X0200_DT2_0X81.Age = JT808BinaryExtensions.ReadUInt32Little(bytes, ref offset);
                jT808_0X0200_DT2_0X81.Gender = JT808BinaryExtensions.ReadByteLittle(bytes, ref offset);
                jT808_0X0200_DT2_0X81.MsgNum = JT808BinaryExtensions.ReadUInt16Little(bytes, ref offset);
                readSize = offset;
                return jT808_0X0200_DT2_0X81;
            }

            public int Serialize(ref byte[] bytes, int offset, JT808_0x0200_DT2_0x81 value)
            {
                offset += JT808BinaryExtensions.WriteByteLittle(bytes, offset, value.AttachInfoId);
                offset += JT808BinaryExtensions.WriteByteLittle(bytes, offset, value.AttachInfoLength);
                offset += JT808BinaryExtensions.WriteUInt32Little(bytes, offset, value.Age);
                offset += JT808BinaryExtensions.WriteByteLittle(bytes, offset, value.Gender);
                offset += JT808BinaryExtensions.WriteUInt16Little(bytes, offset, value.MsgNum);
                return offset;
            }
        }
    }
}
