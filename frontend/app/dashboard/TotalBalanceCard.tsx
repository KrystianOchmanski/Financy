import useSWR from "swr";
import { fetcher } from "../lib/fetcher";
import InfoCard from "./InfoCard";

export default function TotalBalanceCard() {
  const { data, error, isLoading, mutate } = useSWR(
    "/account/balance",
    fetcher
  );

  return (
    <InfoCard
      title="Balance"
      amount={data}
      textColorClass="text-brand"
    ></InfoCard>
  );
}
