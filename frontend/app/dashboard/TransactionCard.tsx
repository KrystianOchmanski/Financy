import useSWR from "swr";
import { TransactionCardProps } from "../lib/definitions";
import InfoCard from "./InfoCard";
import { fetcher } from "../lib/fetcher";

export default function TransactionCard({
  title,
  endpoint,
  period,
}: TransactionCardProps) {
  const url =
    period.startDate && period.endDate
      ? `${endpoint}?startDate=${period.startDate}&endDate=${period.endDate}`
      : null;

  const { data, error, isLoading } = useSWR(url, fetcher);

  return <InfoCard title={title} amount={data} />;
}
