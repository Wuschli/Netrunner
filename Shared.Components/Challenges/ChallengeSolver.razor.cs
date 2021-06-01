using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Netrunner.Shared.Components.Challenges
{
    public partial class ChallengeSolver
    {
        private List<List<ChallengeCell>> _data;
        private readonly Random _rnd = new Random((int) DateTime.Now.Ticks);
        private List<int> _values;
        private ChallengeInputType _inputType;

        protected override Task OnInitializedAsync()
        {
            _inputType = ChallengeInputType.Row;
            _values = new List<int>();
            for (int i = 0; i < 10; i++)
            {
                int value;
                do
                {
                    value = _rnd.Next(256);
                } while (_values.Contains(value));

                _values.Add(value);
            }

            _data = new List<List<ChallengeCell>>();
            for (int x = 0; x < 10; x++)
            {
                var row = new List<ChallengeCell>();
                _data.Add(row);
                for (int y = 0; y < 10; y++)
                {
                    var cell = new ChallengeCell
                    {
                        Value = _values[_rnd.Next(_values.Count)],
                        X = x,
                        Y = y,
                        Selected = false,
                        Disabled = false
                    };
                    cell.Disabled = IsCellDisabled(cell, null);
                    row.Add(cell);
                }
            }

            return Task.CompletedTask;
        }

        private Task ClickCell(ChallengeCell lastCell)
        {
            if (lastCell.Selected)
                return Task.CompletedTask;

            _inputType = _inputType == ChallengeInputType.Row ? ChallengeInputType.Column : ChallengeInputType.Row;
            lastCell.Selected = true;

            foreach (var row in _data)
            {
                foreach (var cell in row)
                {
                    cell.Disabled = IsCellDisabled(cell, lastCell);
                    Console.WriteLine($"{cell.X}|{cell.Y}: {(cell.Disabled ? "disabled" : "enabled")}");
                }
            }

            return Task.CompletedTask;
        }

        private bool IsCellDisabled(ChallengeCell cell, ChallengeCell? lastCell)
        {
            if (cell.Selected)
                return true;
            if (cell == lastCell)
                return true;
            var lastX = lastCell?.X ?? 0;
            var lastY = lastCell?.Y ?? 0;
            return _inputType == ChallengeInputType.Row ? lastX != cell.X : lastY != cell.Y;
        }
    }

    public class ChallengeCell
    {
        public int Value { get; init; }
        public int X { get; init; }
        public int Y { get; init; }
        public bool Disabled { get; set; }
        public bool Selected { get; set; }
        public string DisplayValue => $"{Value:X2}";
    }

    public enum ChallengeInputType
    {
        Row,
        Column
    }
}