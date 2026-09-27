import { useEffect, useState } from "react";
import api from "../lib/api";

export default function HelloUser() {
  const [name, setName] = useState<string | undefined>(undefined);

  useEffect(() => {
    const fetchName = async () => {
      const data = (await api.get("auth/name")).data;

      setName(data);
    };

    fetchName();
  }, []);

  return (
    <h1 className="font-semibold text-3xl">
      Hello,{" "}
      {name ? (
        name
      ) : (
        <span className="inline-block bg-gray-100 rounded-xl w-24 h-8 animate-pulse align-middle"></span>
      )}
      !
    </h1>
  );
}
