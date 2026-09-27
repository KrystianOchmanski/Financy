import { formatAmount, InfoCardProps } from "../lib/definitions";

export default function InfoCard({
  title,
  amount,
  textColorClass = "text-gray-950",
}: InfoCardProps) {
  return (
    <div className="p-6 bg-white border border-gray-100 shadow-xs rounded-xl">
      <h2 className="text-[14px] text-gray-600 mb-2">{title}</h2>
      {amount === undefined ? (
        <p className="font-semibold text-4xl text-transparent bg-gray-100 animate-pulse rounded-xl">
          &nbsp; {/* Keeping space while loading */}
        </p>
      ) : (
        <p className={`font-semibold text-4xl ${textColorClass}`}>
          ${formatAmount(amount)}
        </p>
      )}
    </div>
  );
}
