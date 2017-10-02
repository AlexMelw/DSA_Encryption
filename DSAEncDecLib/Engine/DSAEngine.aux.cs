namespace DSAEncDecLib.Engine
{
    using System;
    using System.Numerics;
    using System.Threading;
    using System.Threading.Tasks;
    using AlgorithmHelpers;
    using SpecificTypes;

    partial class DSAEngine
    {
        private async Task<(BigInteger p, BigInteger q)> GeneratePQPairAsync()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;


            (BigInteger p, BigInteger q) = default((BigInteger, BigInteger));

            try
            {
                switch (Environment.ProcessorCount)
                {
                    case 16:
                        (p, q) = await
                            (await Task.WhenAny(
                                    GeneratePQPrimesAsync(1, cancellationToken),
                                    GeneratePQPrimesAsync(2, cancellationToken),
                                    GeneratePQPrimesAsync(3, cancellationToken),
                                    GeneratePQPrimesAsync(4, cancellationToken),
                                    GeneratePQPrimesAsync(5, cancellationToken),
                                    GeneratePQPrimesAsync(6, cancellationToken),
                                    GeneratePQPrimesAsync(7, cancellationToken),
                                    GeneratePQPrimesAsync(8, cancellationToken))
                                .ConfigureAwait(false))
                            .ConfigureAwait(false);
                        break;

                    case 8:
                        (p, q) = await
                            (await Task.WhenAny(
                                    GeneratePQPrimesAsync(1, cancellationToken),
                                    GeneratePQPrimesAsync(2, cancellationToken),
                                    GeneratePQPrimesAsync(3, cancellationToken),
                                    GeneratePQPrimesAsync(4, cancellationToken))
                                .ConfigureAwait(false))
                            .ConfigureAwait(false);
                        break;

                    case 4:
                        (p, q) = await
                            (await Task.WhenAny(
                                    GeneratePQPrimesAsync(1, cancellationToken),
                                    GeneratePQPrimesAsync(2, cancellationToken))
                                .ConfigureAwait(false))
                            .ConfigureAwait(false);
                        break;

                    case 2:
                    case 1:
                    default:
                        (p, q) = await GeneratePQPrimesAsync(1, cancellationToken)
                            .ConfigureAwait(false);
                        break;
                }
                cancellationTokenSource.Cancel();
            }
            finally
            {
                cancellationTokenSource.Dispose();
                await Console.Out.WriteLineAsync("@@@@@@@@@@@@@@@ SUCCESSFULLY CANCELLED @@@@@@@@@@@@@@").ConfigureAwait(false);
            }

            Console.Out.WriteLine(" ------------------- p = {0}", p);
            Console.Out.WriteLine(" ------------------- q = {0}", q);

            return (p, q);
        }

        private static async Task<(BigInteger, BigInteger)> GeneratePQPrimesAsync(int taskId, CancellationToken cancellationToken)
        {
            (BigInteger, BigInteger) pq = await Task.Run(async () =>
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    await Console.Out.WriteLineAsync($"Task {taskId} is canceled before started.").ConfigureAwait(false);
                    cancellationToken.ThrowIfCancellationRequested();
                }

                (BigInteger p, BigInteger q) = default((BigInteger, BigInteger));

                bool found = false;
                while (!found)
                {
                    await CancelIfCancellationRequested(taskId, cancellationToken).ConfigureAwait(false);

                    q = await MaurerAlgorithm.Instance.GetProvablePrimeAsync(160).ConfigureAwait(false);

                    for (int i = 0; i < 4096; i++)
                    {
                        await CancelIfCancellationRequested(taskId, cancellationToken).ConfigureAwait(false);

                        BigInteger m = BigIntegerUtil.RandomPositiveFixedSizeBigInteger(1024);
                        BigInteger mr = m % (2 * q);
                        p = m - mr + 1;

                        if (p.IsProbablyPrime(10))
                        {
                            await Console.Out.WriteLineAsync($"Number of iterations performed: {i + 1}").ConfigureAwait(false);
                            //Console.Out.WriteLine("m = {0}", m);
                            //Console.Out.WriteLine($"m len = {m.ToByteArray().Length * 8}");

                            //Console.Out.WriteLine("mr = {0}", mr);
                            //Console.Out.WriteLine($"mr len = {mr.ToByteArray().Length * 8}");
                            found = true;
                            break;
                        }
                    }
                }

                await Console.Out.WriteLineAsync($"p = {p}").ConfigureAwait(false);
                await Console.Out.WriteLineAsync($"q = {q}").ConfigureAwait(false);
                await Console.Out.WriteLineAsync($"======================= TASK {taskId} COMPLETED =========================\n").ConfigureAwait(false);

                return (p, q);

            }, cancellationToken).ConfigureAwait(false);

            return pq;

            async Task CancelIfCancellationRequested(int cancelledTaskId, CancellationToken issuedCancellationToken)
            {
                if (issuedCancellationToken.IsCancellationRequested)
                {
                    await Console.Out.WriteLineAsync($"Task [ID: {cancelledTaskId}] is canceled.").ConfigureAwait(false);
                    issuedCancellationToken.ThrowIfCancellationRequested();
                }
            }
        }

        private static BigInteger NormalizeY(BigInteger modulus, BigInteger y) => y < 0 ? y + modulus : y;

        private BigInteger ComputeModularMultiplicativeInverse(BigInteger number, BigInteger modulus)
        {
            BigInteger modularMultiplicativeInverse = Extended_GCD(modulus, number);

            modularMultiplicativeInverse = NormalizeY(modulus, modularMultiplicativeInverse);

            return modularMultiplicativeInverse;
        }

        private BigInteger ComputeV(BigInteger u1, BigInteger u2)
        {
            var leftOperand = BigInteger.ModPow(PublicKey.Alpha, u1, PublicKey.P);
            var rightOperand = BigInteger.ModPow(PublicKey.Beta, u2, PublicKey.P);

            var innerMod = BigInteger.ModPow(leftOperand * rightOperand, 1, PublicKey.P);
            var outerMod = BigInteger.ModPow(innerMod, 1, PublicKey.Q);

            return outerMod;
        }

        private BigInteger ComputeFirstAuxiliaryNumber(byte[] hashOfSignedData, DSASignature signature,
            BigInteger modularMultiplicativeInversS)
        {
            var hash = new BigInteger(hashOfSignedData);

            BigInteger u1 = BigInteger.ModPow(
                modularMultiplicativeInversS * hash,
                1,
                PublicKey.Q);

            return u1;
        }

        private BigInteger ComputeSecondAuxiliaryNumber(DSASignature signature, BigInteger modularMultiplicativeInversS)
        {
            BigInteger u2 = BigInteger.ModPow(
                modularMultiplicativeInversS * signature.R,
                1,
                PublicKey.Q);

            return u2;
        }

        private BigInteger GenerateAlpha(BigInteger p, BigInteger q)
        {
            BigInteger alpha;
            do
            {
                var g = BigIntegerUtil.NextBigInteger(2, p);
                alpha = BigInteger.ModPow(g, (p - 1) / q, p);
            } while (alpha == 1);
            return alpha;
        }

        private static BigInteger Extended_GCD(BigInteger A, BigInteger B)
        {
            bool reverse = false;

            //if A less than B, switch them
            if (A < B)
            {
                Swap(ref A, ref B);
                reverse = true;
            }

            BigInteger r = B;
            BigInteger q = 0;

            BigInteger x0 = 1;
            BigInteger y0 = 0;

            BigInteger x1 = 0;
            BigInteger y1 = 1;

            BigInteger x = 0;
            BigInteger y = 0;

            while (A % B != 0)
            {
                r = A % B;
                q = A / B;

                x = x0 - q * x1;
                y = y0 - q * y1;

                x0 = x1;
                y0 = y1;

                x1 = x;
                y1 = y;

                A = B;
                B = r;
            }

            var resultR = r;

            return reverse ? x : y;

            void Swap(ref BigInteger X, ref BigInteger Y)
            {
                var temp = X;
                X = Y;
                Y = temp;
            }
        }
    }
}