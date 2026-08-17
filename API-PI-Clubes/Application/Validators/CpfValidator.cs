namespace API_PI_Clubes.Application.Validators
{
    public static class CpfValidator
    {
        public static bool IsValid(string? cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf)) return false;

            cpf = new string(cpf.Where(char.IsDigit).ToArray());

            if (cpf.Length != 11) return false;
            if (new string(cpf[0], 11) == cpf) return false; 

            var numbers = cpf.Select(c => c - '0').ToArray();

            int CalcDigit(int count)
            {
                int sum = 0;
                for (int i = 0; i < count; i++)
                    sum += numbers[i] * (count + 1 - i);

                int rest = sum % 11;
                return rest < 2 ? 0 : 11 - rest;
            }

            return CalcDigit(9) == numbers[9] && CalcDigit(10) == numbers[10];
        }

        public static string Normalize(string cpf) =>
            new string(cpf.Where(char.IsDigit).ToArray());
    }
}