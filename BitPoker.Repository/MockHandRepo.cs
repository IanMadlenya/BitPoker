﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitPoker.Models;

namespace BitPoker.Repository
{
    public class MockHandRepo : IHandRepository
    {
        private PlayerInfo alice;
        private PlayerInfo bob;

        private const String alice_wif = "93Loqe8T3Qn3fCc87AiJHYHJfFFMLy6YuMpXzffyFsiodmAMCZS";
        private const String bob_wif = "91yMBYURGqd38spSA1ydY6UjqWiyD1SBGJDuqPPfRWcpG53T672";
        private const String carol_wif = "91rahqyxZb6R1MMq2rdYomfB8GWsLVqkBMHrUnaepxks73KgfaQ";

        private NBitcoin.BitcoinSecret alice_secret = new NBitcoin.BitcoinSecret(alice_wif, NBitcoin.Network.TestNet);
        private NBitcoin.BitcoinSecret bob_secret = new NBitcoin.BitcoinSecret(bob_wif, NBitcoin.Network.TestNet);
        private NBitcoin.BitcoinSecret carol_secret = new NBitcoin.BitcoinSecret(carol_wif, NBitcoin.Network.TestNet);

        private const String alice_address = "msPJhg9GPzMN6twknwmSQvrUKZbZnk51Tv";
        private const String bob_address = "mhSW3EUNoVkD1ZQV1ZpnxdRMBjo648enyo";

        public MockHandRepo()
        {
            alice = new PlayerInfo()
            {
                BitcoinAddress = alice_address,
                LastSeen = DateTime.UtcNow.AddSeconds(-5),
                IPAddress = "https://www.bitpoker.io/api/players/msPJhg9GPzMN6twknwmSQvrUKZbZnk51Tv",
                Latency = new TimeSpan(0, 0, 0, 0, 200)
            };

            bob = new PlayerInfo()
            {
                BitcoinAddress = bob_address,
                LastSeen = DateTime.UtcNow.AddSeconds(-1),
                IPAddress = "https://www.bitpoker.io/api/players/mhSW3EUNoVkD1ZQV1ZpnxdRMBjo648enyo",
                Latency = new TimeSpan(0, 0, 0, 0, 200)
            };
        }

        public MockHandRepo(IPlayerRepository playerRepo)
        {
            alice = playerRepo.Find("msPJhg9GPzMN6twknwmSQvrUKZbZnk51Tv");
            alice = playerRepo.Find("mhSW3EUNoVkD1ZQV1ZpnxdRMBjo648enyo");
        }

        public void Add(Hand entity)
        {
        }

        public IEnumerable<Hand> All()
        {
            throw new NotImplementedException();
        }

        public Hand Find(Guid id)
        {
            //Hand ID
            switch (id.ToString().ToLower())
            {
                //Heads up as per readme
                //0.01 / 0.02
                case "398b5fe2-da27-4772-81ce-37fa615719b5":
                    PlayerInfo[] players = new PlayerInfo[2];
                    players[0] = alice;
                    players[1] = bob;

                    Hand hand = new Hand(players)
                    {
                        Id = id
                    };

                    //SMALL BLIND
                    Models.Messages.ActionMessage sb = new Models.Messages.ActionMessage()
                    {
                        Action = "SMALL BLIND",
                        Amount = 50000,
                        Id = new Guid("47b466e4-c852-49f3-9a6d-5e59c62a98b6"),
                        TableId = new Guid("bf368921-346a-42d8-9cb8-621f9cad5e16"),
                        HandId = id,
                        BitcoinAddress = alice_address,
                        Index = 0,
                        TimeStamp = new DateTime(2016, 08, 17, 0, 0, 0)
                    };

                    //Sign
                    sb.Signature = alice_secret.PrivateKey.SignMessage(sb.ToString());
                    hand.AddMessage(sb);

                    //BIG BLIND
                    //hash with signature
                    Byte[] data = NBitcoin.DataEncoders.Encoders.ASCII.DecodeData(sb.ToString());
                    Byte[] hash = NBitcoin.Crypto.Hashes.SHA256(data);

                    Models.Messages.ActionMessage bb = new Models.Messages.ActionMessage()
                    {
                        Action = "BIG BLIND",
                        Amount = 100000,
                        Id = new Guid("a29bc370-9492-4b60-ad4f-7c7513064383"),
                        TableId = new Guid("bf368921-346a-42d8-9cb8-621f9cad5e16"),
                        HandId = id,
                        BitcoinAddress = bob_address,
                        Index = 1,
                        TimeStamp = new DateTime(2016, 08, 17, 0, 0, 10),
                        PreviousHash = NBitcoin.DataEncoders.Encoders.Hex.EncodeData(data)
                    };

                    bb.Signature = bob_secret.PrivateKey.SignMessage(bb.ToString());
                    hand.AddMessage(bb);

                    //Reset
                    data = null;
                    hash = null;

                    data = NBitcoin.DataEncoders.Encoders.ASCII.DecodeData(sb.ToString());
                    hash = NBitcoin.Crypto.Hashes.SHA256(data);

                    Models.Messages.ActionMessage sb_call = new Models.Messages.ActionMessage()
                    {
                        Action = "CALL",
                        Amount = 50000,
                        Id = new Guid("e299ebc5-b50f-425e-b839-cb69ef69a12e"),
                        TableId = new Guid("bf368921-346a-42d8-9cb8-621f9cad5e16"),
                        HandId = id,
                        BitcoinAddress = alice_address,
                        Index = 2,
                        TimeStamp = new DateTime(2016, 08, 17, 0, 0, 20),
                        PreviousHash = NBitcoin.DataEncoders.Encoders.Hex.EncodeData(data)
                    };

                    sb_call.Signature = alice_secret.PrivateKey.SignMessage(sb_call.ToString());
                    hand.AddMessage(sb_call);

                    data = null;
                    hash = null;

                    //hash with signature
                    data = NBitcoin.DataEncoders.Encoders.ASCII.DecodeData(sb_call.ToString());
                    hash = NBitcoin.Crypto.Hashes.SHA256(data);

                    Models.Messages.ActionMessage option = new Models.Messages.ActionMessage()
                    {
                        Action = "CHECK",
                        Id = new Guid("54c5c3c1-306a-4f1b-863c-aba29b22cb5c"),
                        TableId = new Guid("bf368921-346a-42d8-9cb8-621f9cad5e16"),
                        HandId = id,
                        BitcoinAddress = bob_address,
                        Index = 1,
                        TimeStamp = new DateTime(2016, 08, 17, 0, 0, 30),
                        PreviousHash = NBitcoin.DataEncoders.Encoders.Hex.EncodeData(data)
                    };

                    option.Signature = bob_secret.PrivateKey.SignMessage(option.ToString());
                    hand.AddMessage(option);

                    //Reset
                    data = null;
                    hash = null;

                    //FLOP
                    data = NBitcoin.DataEncoders.Encoders.ASCII.DecodeData(option.ToString());
                    hash = NBitcoin.Crypto.Hashes.SHA256(data);

                    Models.Messages.ActionMessage pre_flop_bet = new Models.Messages.ActionMessage()
                    {
                        Action = "BET",
                        Amount = 100000,
                        Id = new Guid(""),
                        TableId = new Guid("bf368921-346a-42d8-9cb8-621f9cad5e16"),
                        HandId = id,
                        BitcoinAddress = alice_address,
                        Index = 1,
                        TimeStamp = new DateTime(2016, 08, 17, 0, 0, 40),
                        PreviousHash = NBitcoin.DataEncoders.Encoders.Hex.EncodeData(data)
                    };

                    pre_flop_bet.Signature = bob_secret.PrivateKey.SignMessage(pre_flop_bet.ToString());
                    hand.AddMessage(pre_flop_bet);

                    //Reset
                    data = null;
                    hash = null;

                    data = NBitcoin.DataEncoders.Encoders.ASCII.DecodeData(pre_flop_bet.ToString());
                    hash = NBitcoin.Crypto.Hashes.SHA256(data);

                    Models.Messages.ActionMessage pre_flop_call = new Models.Messages.ActionMessage()
                    {
                        Action = "CALL",
                        Amount = 50000,
                        Id = new Guid(""),
                        TableId = new Guid("bf368921-346a-42d8-9cb8-621f9cad5e16"),
                        HandId = id,
                        BitcoinAddress = bob_address,
                        Index = 2,
                        TimeStamp = new DateTime(2016, 08, 17, 0, 0, 50),
                        PreviousHash = NBitcoin.DataEncoders.Encoders.Hex.EncodeData(data)
                    };

                    pre_flop_call.Signature = alice_secret.PrivateKey.SignMessage(pre_flop_call.ToString());
                    hand.AddMessage(pre_flop_call);

                    //Reset
                    data = null;
                    hash = null;

                    //TURN
                    data = NBitcoin.DataEncoders.Encoders.ASCII.DecodeData(pre_flop_call.ToString());
                    hash = NBitcoin.Crypto.Hashes.SHA256(data);

                    Models.Messages.ActionMessage turn_bet = new Models.Messages.ActionMessage()
                    {
                        Action = "BET",
                        Amount = 50000,
                        Id = new Guid(""),
                        TableId = new Guid("bf368921-346a-42d8-9cb8-621f9cad5e16"),
                        HandId = id,
                        BitcoinAddress = alice_address,
                        Index = 2,
                        TimeStamp = new DateTime(2016, 08, 17, 0, 1, 0),
                        PreviousHash = NBitcoin.DataEncoders.Encoders.Hex.EncodeData(data)
                    };

                    turn_bet.Signature = alice_secret.PrivateKey.SignMessage(turn_bet.ToString());
                    hand.AddMessage(turn_bet);

                    //Reset
                    data = null;
                    hash = null;

                    data = NBitcoin.DataEncoders.Encoders.ASCII.DecodeData(turn_bet.ToString());
                    hash = NBitcoin.Crypto.Hashes.SHA256(data);

                    Models.Messages.ActionMessage turn_call = new Models.Messages.ActionMessage()
                    {
                        Action = "CALL",
                        Amount = 50000,
                        Id = new Guid(""),
                        TableId = new Guid("bf368921-346a-42d8-9cb8-621f9cad5e16"),
                        HandId = id,
                        BitcoinAddress = bob_address,
                        Index = 2,
                        TimeStamp = new DateTime(2016, 08, 17, 0, 1, 10),
                        PreviousHash = NBitcoin.DataEncoders.Encoders.Hex.EncodeData(data)
                    };

                    turn_call.Signature = alice_secret.PrivateKey.SignMessage(turn_call.ToString());
                    hand.AddMessage(turn_call);

                    //Reset
                    data = null;
                    hash = null;

                    //River

                    return hand;
                case "91dacf01-4c4b-4656-912b-2c3a11f6e516":
                    //heads up?
                    break;

            }

            throw new NotImplementedException();
        }

        public void Update(Hand entity)
        {
        }

        public Int32 Save()
        {
            return 0;
        }

        public void Dispose()
        {
        }
    }
}
